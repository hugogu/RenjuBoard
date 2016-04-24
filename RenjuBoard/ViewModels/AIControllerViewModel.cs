using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Prism.Commands;
using Renju.Infrastructure;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;

namespace RenjuBoard.ViewModels
{
    public class AIControllerViewModel : ModelBase
    {
        private readonly ObservableCollection<PieceLine> _previewLines = new ObservableCollection<PieceLine>();
        private readonly IStepController _resolverController;

        public AIControllerViewModel(IStepController aiStepController, OptionsViewModel optionsVM, IGameBoard<IReadOnlyBoardPoint> gameBoard)
        {
            Debug.Assert(aiStepController != null);
            Debug.Assert(aiStepController.CurrentStep == 0);
            _resolverController = aiStepController;
            PauseAICommand = new DelegateCommand(() => aiStepController.Pause(), () => !aiStepController.IsPaused);
            ContinueAICommand = new DelegateCommand(() => aiStepController.Resume(), () => aiStepController.IsPaused);
            NextAIStepComand = new DelegateCommand(() => aiStepController.StepForward(1), () => aiStepController.IsPaused);
            ClearPreviewLinesCommand = new DelegateCommand(() => _previewLines.Clear());
            PreviewLinesCommand = new DelegateCommand<IReadOnlyBoardPoint>(point =>
            {
                _previewLines.Clear();
                _previewLines.AddRange(point.GetRowsOnBoard(gameBoard, true));
            }, p => optionsVM.ShowPreviewLine);
            aiStepController.PropertyChanged += OnResolverControllerPropertyChanged;
        }

        public DelegateCommand PauseAICommand { get; private set; }

        public DelegateCommand NextAIStepComand { get; private set; }

        public DelegateCommand ContinueAICommand { get; private set; }

        public ICommand PreviewLinesCommand { get; private set; }

        public ICommand ClearPreviewLinesCommand { get; private set; }

        public IEnumerable<PieceLine> PreviewLines
        {
            get { return _previewLines; }
        }

        public bool PauseOnStart
        {
            get { return _resolverController.PauseOnStart; }
            set { _resolverController.PauseOnStart = value; }
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
