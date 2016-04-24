using System;
using System.Collections.Generic;
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
using Renju.Infrastructure.Model;
using RenjuBoard.ViewModels;

namespace RenjuBoard
{
    public class MainWindowViewModel : DisposableModelBase
    {
        private readonly IDropResolver _dropResolver;
        private readonly IGamePlayer _aiPlayer;
        private readonly VirtualGameBoard<BoardPoint> _resolvingBoard;

        public MainWindowViewModel(
            IGameBoard<IReadOnlyBoardPoint> gameBoard,
            BoardRecorder boardRecorder,
            IGamePlayer aiPlayer,
            IDropResolver dropResolver,
            AIControllerViewModel controllerVM,
            BoardTimingViewModel timingVM,
            OptionsViewModel optionsVM)
        {
            GameBoard = gameBoard;
            BoardRecorder = boardRecorder;
            OptionsVM = optionsVM;
            TimingVM = timingVM;
            AIControllerVM = controllerVM;
            _resolvingBoard = new VirtualGameBoard<BoardPoint>(GameBoard.Size, BoardPoint.CreateIndexBasedFactory(GameBoard.Size));
            _dropResolver = dropResolver;
            _dropResolver.ResolvingBoard += OnResolvingBoard;
            _aiPlayer = aiPlayer;
            DropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnDroppingPiece);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            LoadCommand = new DelegateCommand(OnLoadCommand);
            ShowOptionsCommand = new DelegateCommand(OnShowOptionsCommand);
            NewGameCommand = new DelegateCommand(OnNewGameCommand);
            AutoDispose(OptionsVM.ObserveProperty(() => OptionsVM.ShowLinesOnBoard)
                                 .Subscribe(_ => OnPropertyChanged(() => Lines)));
            AutoDispose(OptionsVM.ObserveProperty<object>(() => OptionsVM.IsAITimeLimited, () => OptionsVM.AIStepTimeLimit, () => OptionsVM.AITimeLimit)
                                 .Subscribe(_ => ReloadAITimeLimitOptions()));
        }

        [Dependency]
        public IEventAggregator EventAggregator { get; internal set; }

        public IGameBoard<IReadOnlyBoardPoint> GameBoard { get; private set; }

        public ICommand DropPointCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand LoadCommand { get; private set; }

        public ICommand ShowOptionsCommand { get; private set; }

        public ICommand NewGameCommand { get; private set; }

        public BoardRecorder BoardRecorder { get; private set; }

        public AIControllerViewModel AIControllerVM { get; private set; }

        public OptionsViewModel OptionsVM { get; private set; }

        public BoardTimingViewModel TimingVM { get; private set; }

        public IEnumerable<PieceLine> Lines
        {
            get { return OptionsVM.ShowLinesOnBoard ? GameBoard.Lines : new PieceLine[0]; }
        }

        public bool AIFirst
        {
            get { return _aiPlayer.Side == Side.Black; }
            set { _aiPlayer.Side = value ? Side.Black : Side.White; }
        }

        public IEnumerable<IReadOnlyBoardPoint> ResolvingPoints
        {
            get { return _resolvingBoard.Points; }
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
                BoardRecorder.ClearGameBoard();
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
                    foreach (var drop in BoardRecorder.Drops.Concat(BoardRecorder.RedoDrops))
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
    }
}
