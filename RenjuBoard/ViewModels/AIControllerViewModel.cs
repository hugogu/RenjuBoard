using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Renju.AI;
using Renju.Core;
using Renju.Infrastructure;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;

namespace RenjuBoard.ViewModels
{
    public class AIControllerViewModel : ModelBase
    {
        private readonly ObservableCollection<PieceLine> _previewLines = new ObservableCollection<PieceLine>();
        private readonly VirtualGameBoard<BoardPoint> _resolvingBoard;
        private readonly IStepController _resolverController;

        public AIControllerViewModel(IStepController aiStepController, IDropResolver dropResolver, IGameBoard<IReadOnlyBoardPoint> gameBoard)
        {
            Debug.Assert(aiStepController != null);
            Debug.Assert(aiStepController.CurrentStep == 0);
            _resolverController = aiStepController;
            _resolvingBoard = new VirtualGameBoard<BoardPoint>(gameBoard.Size, BoardPoint.CreateIndexBasedFactory(gameBoard.Size));
            dropResolver.ResolvingBoard += OnResolvingBoard;
            PauseAICommand = new DelegateCommand(() => aiStepController.Pause(), () => !aiStepController.IsPaused);
            ContinueAICommand = new DelegateCommand(() => aiStepController.Resume(), () => aiStepController.IsPaused);
            NextAIStepComand = new DelegateCommand(() => aiStepController.StepForward(1), () => aiStepController.IsPaused);
            ClearPreviewLinesCommand = new DelegateCommand(() => _previewLines.Clear());
            PreviewLinesCommand = new DelegateCommand<IReadOnlyBoardPoint>(point =>
                {
                    _previewLines.Clear();
                    _previewLines.AddRange(point.GetRowsOnBoard(gameBoard, true));
                }, p => OptionsVM.ShowPreviewLine);
            aiStepController.PropertyChanged += OnResolverControllerPropertyChanged;
        }

        [Dependency]
        public IGamePlayer AIPlayer { get; internal set; }

        [Dependency]
        public OptionsViewModel OptionsVM { get; internal set; }

        public DelegateCommand PauseAICommand { get; private set; }

        public DelegateCommand NextAIStepComand { get; private set; }

        public DelegateCommand ContinueAICommand { get; private set; }

        public ICommand PreviewLinesCommand { get; private set; }

        public ICommand ClearPreviewLinesCommand { get; private set; }

        public IEnumerable<PieceLine> PreviewLines
        {
            get { return _previewLines; }
        }

        public IEnumerable<IReadOnlyBoardPoint> ResolvingPoints
        {
            get { return _resolvingBoard.Points; }
        }

        public bool PauseOnStart
        {
            get { return _resolverController.PauseOnStart; }
            set { _resolverController.PauseOnStart = value; }
        }

        public bool AIFirst
        {
            get { return AIPlayer.Side == Side.Black; }
            set { AIPlayer.Side = value ? Side.Black : Side.White; }
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

        private void OnResolverControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RunInDispatcher(() =>
            {
                if (e.PropertyName == Reflections.GetMemberName(() => _resolverController.IsPaused))
                {
                    PauseAICommand.RaiseCanExecuteChanged();
                    ContinueAICommand.RaiseCanExecuteChanged();
                    NextAIStepComand.RaiseCanExecuteChanged();
                }
            });
        }
    }
}
