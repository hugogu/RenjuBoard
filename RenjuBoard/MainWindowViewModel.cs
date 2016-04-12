using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Commands;
using Renju.AI;
using Renju.AI.Resolving;
using Renju.AI.Weights;
using Renju.Core;
using Renju.Core.Rules;
using Renju.Infrastructure;
using Renju.Infrastructure.Execution;
using RenjuBoard.ViewModels;

namespace RenjuBoard
{
    public class MainWindowViewModel : ModelBase, IDisposable
    {
        private readonly IGameBoard<IReadOnlyBoardPoint> _gameBoard;
        private readonly ICommand _saveCommand;
        private readonly ICommand _loadCommand;
        private readonly ICommand _dropPointCommand;
        private readonly ICommand _previewLinesCommand;
        private readonly ICommand _clearPreviewLinesCommand;
        private readonly DelegateCommand _undoDropCommand;
        private readonly DelegateCommand _redoDropCommand;
        private readonly IDropSelector _dropSelector = new WeightedDropSelector { RandomEqualSelections = true };
        private readonly IDropResolver _dropResolver;
        private readonly ObservableCollection<PieceLine> _previewLines = new ObservableCollection<PieceLine>();
        private readonly AIGamePlayer _aiPlayer;
        private readonly BoardRecorder _boardRecorder;
        private readonly VirtualGameBoard<BoardPoint> _resolvingBoard;
        private readonly HumanExecutionNotifier _humanExecutionNotifier;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public MainWindowViewModel(int boardSize = 15)
        {
            _gameBoard = new GameBoard(boardSize, new DefaultGameRuleEngine(new IGameRule[]
                         {
                             new FiveWinRule(),
                             new BlackForbiddenRules()
                         }));
            _resolvingBoard = new VirtualGameBoard<BoardPoint>(boardSize, BoardPoint.CreateIndexBasedFactory(boardSize));
            _boardRecorder = new BoardRecorder(_gameBoard);
            _dropResolver = new WinRateGameResolver(_dropSelector);
            _aiPlayer = new AIGamePlayer(_dropResolver) { Side = Side.White, Board = _gameBoard };
            _dropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnDroppingPiece);
            _previewLinesCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnPreviewPointCommand, p => ShowLines);
            _clearPreviewLinesCommand = new DelegateCommand(() => _previewLines.Clear());
            _undoDropCommand = new DelegateCommand(() => _boardRecorder.UndoDrop(), () => _boardRecorder.CanUndo);
            _redoDropCommand = new DelegateCommand(() => _boardRecorder.RedoDrop(), () => _boardRecorder.CanRedo);
            _saveCommand = new DelegateCommand(OnSaveCommand);
            _loadCommand = new DelegateCommand(OnLoadCommand);
            _boardRecorder.PropertyChanged += OnBoardRecorderPropertyChanged;
            _dropResolver.ResolvingBoard += OnResolvingBoard;
            _humanExecutionNotifier = new HumanExecutionNotifier(_gameBoard, this);
            AIControllerVM = new AIControllerViewModel(_dropResolver);
            _disposables.Add(Observable.Merge(_humanExecutionNotifier.ExecutionTimer.ObserveAnyProperties(),
                                              _dropResolver.ExecutionTimer.ObserveAnyProperties())
                                       .Subscribe(args =>
                                       {
                                           RaisePropertyChanged(() => BlackTime);
                                           RaisePropertyChanged(() => WhiteTime);
                                       }));
            _disposables.Add(_humanExecutionNotifier);
        }

        public ICommand DropPointCommand
        {
            get { return _dropPointCommand; }
        }

        public ICommand PreviewLinesCommand
        {
            get { return _previewLinesCommand; }
        }

        public ICommand ClearPreviewLinesCommand
        {
            get { return _clearPreviewLinesCommand; }
        }

        public ICommand UndoCommand
        {
            get { return _undoDropCommand; }
        }

        public ICommand RedoCommand
        {
            get { return _redoDropCommand; }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return _gameBoard.Points; }
        }

        public bool ShowLines { get; set; } = true;

        public bool ShowAISteps { get; set; } = true;

        public AIControllerViewModel AIControllerVM { get; private set; }

        public IEnumerable<PieceLine> PreviewLines
        {
            get { return _previewLines; }
        }

        public int BoardSize
        {
            get { return _gameBoard.Size; }
        }

        public bool AIFirst
        {
            get { return _aiPlayer.Side == Side.Black; }
            set { _aiPlayer.Side = value ? Side.Black : Side.White; }
        }

        public TimeSpan BlackTime
        {
            get { return AIFirst ? _dropResolver.ExecutionTimer.TotalExecutionTime : _humanExecutionNotifier.ExecutionTimer.TotalExecutionTime; }
        }

        public TimeSpan WhiteTime
        {
            get { return AIFirst ? _humanExecutionNotifier.ExecutionTimer.TotalExecutionTime : _dropResolver.ExecutionTimer.TotalExecutionTime; }
        }

        public IEnumerable<IReadOnlyBoardPoint> ResolvingPoints
        {
            get { return _resolvingBoard.Points; }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        internal void ClearGameBoard()
        {
            while (_boardRecorder.CanUndo)
                _boardRecorder.UndoDrop();
        }

        private void OnResolvingBoard(object sender, ResolvingBoardEventArgs e)
        {
            foreach (var showingPoint in _resolvingBoard.Points)
            {
                var virtualPoint = e.Board == null ? null : e.Board[showingPoint.Position];
                if (virtualPoint is VirtualBoardPoint && ShowAISteps)
                {
                    showingPoint.Index = virtualPoint.Index;
                    showingPoint.Status = virtualPoint.Status;
                }
                else if (showingPoint.Index.HasValue)
                {
                    showingPoint.ResetToEmpty();
                }
            }
        }

        private void OnDroppingPiece(IReadOnlyBoardPoint point)
        {
            try
            {
                _gameBoard.Drop(point.Position, OperatorType.Human);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Renju Error", MessageBoxButton.OK);
            }
        }

        private void OnPreviewPointCommand(IReadOnlyBoardPoint point)
        {
            _previewLines.Clear();
            _previewLines.AddRange(point.GetLinesOnBoard(_gameBoard, true));
        }

        private async void OnLoadCommand()
        {
            var loadFile = AskForLoadingLocation();
            if (loadFile != null)
            {
                ClearGameBoard();
                using (var streamReader = new StreamReader(File.OpenRead(loadFile)))
                {
                    var converter = TypeDescriptor.GetConverter(typeof(PieceDrop));
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync();
                        var drop = converter.ConvertFromString(line) as PieceDrop;
                        _gameBoard.Drop(drop, OperatorType.Loading);
                    }
                }
            }
        }

        private void OnSaveCommand()
        {
            var saveFile = AskForSavingLocation();
            if (saveFile != null)
            {
                using (var streamWriter = new StreamWriter(File.OpenWrite(saveFile)))
                {
                    var converter = TypeDescriptor.GetConverter(typeof(PieceDrop));
                    foreach (var drop in _boardRecorder.Drops.Concat(_boardRecorder.RedoDrops))
                    {
                        streamWriter.WriteLine(converter.ConvertToString(drop));
                    }
                }
            }
        }

        private string AskForLoadingLocation()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ValidateNames = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Renju Game (*.renju) |*.renju| All Files |*.*";
            openFileDialog.Title = "Select a file for Renju Game";
            if (openFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private string AskForSavingLocation()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ValidateNames = true;
            saveFileDialog.Filter = "Renju Game (*.renju) |*.renju| All Files |*.*";
            saveFileDialog.Title = "Select a file for Renju Game";
            if (saveFileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        private void OnBoardRecorderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RunInDispatcher(new Action(() =>
            {
                _undoDropCommand.RaiseCanExecuteChanged();
                _redoDropCommand.RaiseCanExecuteChanged();
            }));
        }

        private class HumanExecutionNotifier : ReportExecutionObject
        {
            private readonly AIGamePlayer _aiPlayer;

            public HumanExecutionNotifier(IGameBoard<IReadOnlyBoardPoint> board, MainWindowViewModel vm)
            {
                _aiPlayer = vm._aiPlayer;
                board.PieceDropped += OnBoardPieceDropped;
            }

            private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
            {
                var board = sender as IGameBoard<IReadOnlyBoardPoint>;
                if (board.DroppedPoints.Last().Status == _aiPlayer.Side)
                {
                    RaiseStartedEvent();
                }
                else if (board.DroppedPoints.Last().Status != null)
                {
                    RaiseFinishedEvent();
                }
            }
        }
    }
}
