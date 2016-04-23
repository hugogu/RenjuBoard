using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Renju.AI;
using Renju.Core;
using Renju.Infrastructure;
using Renju.Infrastructure.Events;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;
using RenjuBoard.ViewModels;

namespace RenjuBoard
{
    public class MainWindowViewModel : DisposableModelBase
    {
        private readonly ICommand _saveCommand;
        private readonly ICommand _loadCommand;
        private readonly ICommand _dropPointCommand;
        private readonly ICommand _showOptionsCommand;
        private readonly ICommand _previewLinesCommand;
        private readonly ICommand _clearPreviewLinesCommand;
        private readonly ICommand _newGameCommand;
        private readonly DelegateCommand _undoDropCommand;
        private readonly DelegateCommand _redoDropCommand;
        private readonly IDropResolver _dropResolver;
        private readonly ObservableCollection<PieceLine> _previewLines = new ObservableCollection<PieceLine>();
        private readonly IGamePlayer _aiPlayer;
        private readonly BoardRecorder _boardRecorder;
        private readonly VirtualGameBoard<BoardPoint> _resolvingBoard;
        private readonly HumanExecutionNotifier _humanExecutionNotifier;

        public MainWindowViewModel(
            IGameBoard<IReadOnlyBoardPoint> gameBoard,
            BoardRecorder boardRecorder,
            IGamePlayer aiPlayer,
            IDropResolver dropResolver)
        {
            GameBoard = gameBoard;
            _boardRecorder = boardRecorder;
            _resolvingBoard = new VirtualGameBoard<BoardPoint>(BoardSize, BoardPoint.CreateIndexBasedFactory(BoardSize));
            _dropResolver = dropResolver;
            _aiPlayer = aiPlayer;
            _dropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnDroppingPiece);
            _previewLinesCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnPreviewPointCommand, p => OptionsVM.ShowPreviewLine);
            _clearPreviewLinesCommand = new DelegateCommand(() => _previewLines.Clear());
            _undoDropCommand = new DelegateCommand(() => _boardRecorder.UndoDrop(), () => _boardRecorder.CanUndo);
            _redoDropCommand = new DelegateCommand(() => _boardRecorder.RedoDrop(), () => _boardRecorder.CanRedo);
            _saveCommand = new DelegateCommand(OnSaveCommand);
            _loadCommand = new DelegateCommand(OnLoadCommand);
            _showOptionsCommand = new DelegateCommand(OnShowOptionsCommand);
            _newGameCommand = new DelegateCommand(OnNewGameCommand);
            _boardRecorder.PropertyChanged += OnBoardRecorderPropertyChanged;
            _dropResolver.ResolvingBoard += OnResolvingBoard;
            _humanExecutionNotifier = new HumanExecutionNotifier(GameBoard, this);
            OptionsVM = new OptionsViewModel();
            AIControllerVM = new AIControllerViewModel(new ExecutionStepController(_dropResolver));
            AutoDispose(AIControllerVM);
            AutoDispose(Observable.Merge(_humanExecutionNotifier.ExecutionTimer.ObserveAnyProperties(),
                                         _dropResolver.ExecutionTimer.ObserveAnyProperties())
                                  .Subscribe(args =>
                                  {
                                      OnPropertyChanged(() => BlackTime);
                                      OnPropertyChanged(() => WhiteTime);
                                  }));
            AutoDispose(_humanExecutionNotifier);
            AutoDispose(OptionsVM.ObserveProperty(() => OptionsVM.ShowLinesOnBoard)
                                 .Subscribe(_ => OnPropertyChanged(() => Lines)));
            AutoDispose(OptionsVM.ObserveProperty<object>(() => OptionsVM.IsAITimeLimited, () => OptionsVM.AIStepTimeLimit, () => OptionsVM.AITimeLimit)
                                 .Subscribe(_ => ReloadAITimeLimitOptions()));
        }

        [Dependency]
        public IEventAggregator EventAggregator { get; internal set; }

        public IGameBoard<IReadOnlyBoardPoint> GameBoard { get; private set; }

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

        public ICommand ShowOptionsCommand
        {
            get { return _showOptionsCommand; }
        }

        public ICommand NewGameCommand
        {
            get { return _newGameCommand; }
        }

        public IEnumerable<IReadOnlyBoardPoint> Points
        {
            get { return GameBoard.Points; }
        }

        public AIControllerViewModel AIControllerVM { get; private set; }

        public OptionsViewModel OptionsVM { get; private set; }

        public IEnumerable<PieceLine> PreviewLines
        {
            get { return _previewLines; }
        }

        public IEnumerable<PieceLine> Lines
        {
            get { return OptionsVM.ShowLinesOnBoard ? GameBoard.Lines : new PieceLine[0]; }
        }

        public int BoardSize
        {
            get { return GameBoard.Size; }
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

        internal void ClearGameBoard()
        {
            while (_boardRecorder.CanUndo)
                _boardRecorder.UndoDrop();
        }

        private void OnNewGameCommand()
        {
            EventAggregator.GetEvent<StartNewGameEvent>().Publish(NewGameOptions.Default);
        }

        private void OnResolvingBoard(object sender, ResolvingBoardEventArgs e)
        {
            foreach (var showingPoint in _resolvingBoard.Points)
            {
                var virtualPoint = e.Board == null ? null : e.Board[showingPoint.Position];
                if (virtualPoint is VirtualBoardPoint && OptionsVM.ShowAISteps)
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
                GameBoard.Drop(point.Position, OperatorType.Human);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Renju Error", MessageBoxButton.OK);
            }
        }

        private void OnPreviewPointCommand(IReadOnlyBoardPoint point)
        {
            _previewLines.Clear();
            _previewLines.AddRange(point.GetRowsOnBoard(GameBoard, true));
        }

        private void ReloadAITimeLimitOptions()
        {
            if (OptionsVM.IsAITimeLimited)
            {
                _dropResolver.MaxTotalTime = TimeSpan.FromMilliseconds(OptionsVM.AITimeLimit);
                _dropResolver.MaxStepTime = TimeSpan.FromMilliseconds(OptionsVM.AIStepTimeLimit);
            }
            else
            {
                _dropResolver.MaxTotalTime = TimeSpan.MaxValue;
                _dropResolver.MaxStepTime = TimeSpan.MaxValue;
            }
        }

        private void OnShowOptionsCommand()
        {
            var optionsCopy = new OptionsViewModel(OptionsVM);
            var optionsWindow = new Window()
            {
                Owner = Application.Current.MainWindow,
                Title = "Renju Options",
                Content = optionsCopy,
                MinHeight = 200,
                MinWidth = 300,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStyle = WindowStyle.SingleBorderWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (optionsWindow.ShowDialog() == true)
            {
                OptionsVM.CopyFrom(optionsCopy);
            }
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
                        GameBoard.Drop(drop, OperatorType.Loading);
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
            private readonly IGamePlayer _aiPlayer;

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
