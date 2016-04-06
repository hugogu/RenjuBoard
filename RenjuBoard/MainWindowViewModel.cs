using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Commands;
using Renju.AI;
using Renju.AI.Resolving;
using Renju.AI.Weights;
using Renju.Core;
using Renju.Core.Rules;
using Runju.Infrastructure;

namespace RenjuBoard
{
    public class MainWindowViewModel : ModelBase
    {
        private readonly IGameBoard _gameBoard;
        private readonly ICommand _saveCommand;
        private readonly ICommand _loadCommand;
        private readonly ICommand _dropPointCommand;
        private readonly DelegateCommand _undoDropCommand;
        private readonly DelegateCommand _redoDropCommand;
        private readonly AIGamePlayer _aiPlayer = new AIGamePlayer(new WinRateGameResolver(new WeightedDropSelector() { RandomEqualSelections = true }));
        private readonly BoardRecorder _boardRecorder;

        public MainWindowViewModel()
        {
            _gameBoard = new GameBoard(15, new DefaultGameRuleEngine(new IGameRule[]
            {
                new FiveWinRule()
            }));
            _boardRecorder = new BoardRecorder(_gameBoard);
            _aiPlayer.Side = Side.White;
            _aiPlayer.Board = _gameBoard;
            _dropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(point => _gameBoard.Drop(point.Position, OperatorType.Human));
            _undoDropCommand = new DelegateCommand(() => _boardRecorder.UndoDrop(), () => _boardRecorder.CanUndo);
            _redoDropCommand = new DelegateCommand(() => _boardRecorder.RedoDrop(), () => _boardRecorder.CanRedo);
            _saveCommand = new DelegateCommand(OnSaveCommand);
            _loadCommand = new DelegateCommand(OnLoadCommand);
            _boardRecorder.PropertyChanged += OnBoardRecorderPropertyChanged;
        }

        public ICommand DropPointCommand
        {
            get { return _dropPointCommand; }
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

        public int BoardSize
        {
            get { return _gameBoard.Size; }
        }

        public bool AIFirst
        {
            get { return _aiPlayer.Side == Side.Black; }
            set { _aiPlayer.Side = value ? Side.Black : Side.White; }
        }

        internal void ClearGameBoard()
        {
            while(_boardRecorder.CanUndo)
                _boardRecorder.UndoDrop();
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
                    foreach(var drop in _boardRecorder.Drops.Concat(_boardRecorder.RedoDrops))
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
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _undoDropCommand.RaiseCanExecuteChanged();
                _redoDropCommand.RaiseCanExecuteChanged();
            }));
        }
    }
}
