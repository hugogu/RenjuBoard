using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
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
        public MainWindowViewModel(IDropResolver dropResolver, OptionsViewModel optionsVM)
        {
            OptionsVM = optionsVM;
            DropPointCommand = new DelegateCommand<IReadOnlyBoardPoint>(OnDroppingPiece);
            NewGameCommand = new DelegateCommand(OnNewGameCommand);
            AutoDispose(OptionsVM.ObserveProperty(() => OptionsVM.ShowLinesOnBoard)
                                 .Subscribe(_ => OnPropertyChanged(() => Lines)));
            AutoDispose(OptionsVM.ObserveProperty<object>(() => OptionsVM.IsAITimeLimited, () => OptionsVM.AIStepTimeLimit, () => OptionsVM.AITimeLimit)
                                 .Subscribe(_ =>
                                 {
                                     dropResolver.MaxTotalTime = OptionsVM.IsAITimeLimited ? TimeSpan.FromMilliseconds(OptionsVM.AITimeLimit) : TimeSpan.MaxValue;
                                     dropResolver.MaxStepTime = OptionsVM.IsAITimeLimited ? TimeSpan.FromMilliseconds(OptionsVM.AIStepTimeLimit) : TimeSpan.MaxValue;
                                 }));
        }

        [Dependency]
        public BoardRecorder BoardRecorder { get; internal set; }

        [Dependency]
        public AIControllerViewModel AIControllerVM { get; internal set; }

        [Dependency]
        public BoardTimingViewModel TimingVM { get; internal set; }

        [Dependency]
        public SaveAndLoadViewModel SaveLoadVM { get; internal set; }

        [Dependency]
        public IEventAggregator EventAggregator { get; internal set; }

        [Dependency]
        public IGameBoard<IReadOnlyBoardPoint> GameBoard { get; internal set; }

        public ICommand DropPointCommand { get; private set; }

        public ICommand NewGameCommand { get; private set; }

        public OptionsViewModel OptionsVM { get; private set; }

        public IEnumerable<PieceLine> Lines
        {
            get { return OptionsVM.ShowLinesOnBoard ? GameBoard.Lines : new PieceLine[0]; }
        }

        private void OnNewGameCommand()
        {
            EventAggregator.GetEvent<StartNewGameEvent>().Publish(NewGameOptions.Default);
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
    }
}
