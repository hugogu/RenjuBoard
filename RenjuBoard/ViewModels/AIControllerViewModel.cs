using System;
using System.ComponentModel;
using System.Windows.Input;
using Prism.Commands;
using Renju.Infrastructure;
using Renju.Infrastructure.Execution;

namespace RenjuBoard.ViewModels
{
    public class AIControllerViewModel : DisposableModelBase
    {
        private readonly DelegateCommand _pauseAICommand;
        private readonly DelegateCommand _continueAICommand;
        private readonly DelegateCommand _nextAIStepCommand;
        private readonly IStepController _resolverController;

        public AIControllerViewModel(IReportExecutionStatus executor)
        {
            _resolverController = new ExecutionStepController(executor);
            _pauseAICommand = new DelegateCommand(() => _resolverController.Pause(), () => !_resolverController.IsPaused);
            _continueAICommand = new DelegateCommand(() => _resolverController.Resume(), () => _resolverController.IsPaused);
            _nextAIStepCommand = new DelegateCommand(() => _resolverController.StepForward(1), () => _resolverController.IsPaused);
            _resolverController.PropertyChanged += OnResolverControllerPropertyChanged;
            AutoDispose(_resolverController as IDisposable);
        }

        public ICommand PauseAICommand
        {
            get { return _pauseAICommand; }
        }

        public ICommand NextAIStepComand
        {
            get { return _nextAIStepCommand; }
        }

        public ICommand ContinueAICommand
        {
            get { return _continueAICommand; }
        }

        public bool PauseOnStart
        {
            get { return _resolverController.PauseOnStart; }
            set { _resolverController.PauseOnStart = value; }
        }

        private void OnResolverControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RunInDispatcher(new Action(() =>
            {
                if (e.PropertyName == "IsPaused")
                {
                    _pauseAICommand.RaiseCanExecuteChanged();
                    _continueAICommand.RaiseCanExecuteChanged();
                    _nextAIStepCommand.RaiseCanExecuteChanged();
                }
            }));
        }
    }
}
