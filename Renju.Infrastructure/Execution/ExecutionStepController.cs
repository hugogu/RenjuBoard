using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.Unity;
using Renju.Infrastructure.Model;

namespace Renju.Infrastructure.Execution
{
    public class ExecutionStepController : DisposableModelBase, IStepController
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(true);
        private readonly IReportExecutionStatus _executor;
        private bool _isInSteppingMode = false;
        private int _executionID;
        private int _allowedSteps;

        public ExecutionStepController([Dependency("ai")]IReportExecutionStatus executor)
        {
            _executor = executor;
            executor.Started += OnExecutorStarted;
            executor.Finished += OnExecutorFinished;
            executor.StepFinished += OnExecutorFinishedStep;
            AutoCallOnDisposing(() =>
            {
                executor.Started -= OnExecutorStarted;
                executor.Finished -= OnExecutorFinished;
                executor.StepFinished -= OnExecutorFinishedStep;
            });
            AutoDispose(_event);
        }

        public int CurrentStep { get; private set; }

        public bool IsPaused { get { return !_event.WaitOne(0); } }

        [Dependency]
        public GameOptions Options { get; set; }

        public void Resume()
        {
            _isInSteppingMode = false;
            SetEvent();
        }

        public void Pause()
        {
            _isInSteppingMode = true;
            ResetEvent();
        }

        public void StepBackward(int steps)
        {
            throw new NotSupportedException();
        }

        public void StepForward(int steps)
        {
            _allowedSteps = steps;
            SetEvent();
        }

        protected virtual void SetEvent()
        {
            _executor.ExecutionTimer.Resume();
            _event.Set();
            OnPropertyChanged(() => IsPaused);
        }

        protected virtual void ResetEvent()
        {
            _event.Reset();
            _executor.ExecutionTimer.Pause();
            OnPropertyChanged(() => IsPaused);
        }

        private void OnExecutorFinishedStep(object sender, EventArgs e)
        {
            ValidateCurrentThreadID();
            CurrentStep++;
            _event.WaitOne();
            if (--_allowedSteps == 0 && _isInSteppingMode)
                ResetEvent();
        }

        private void OnExecutorFinished(object sender, EventArgs e)
        {
            ValidateCurrentThreadID();
            SetEvent();
        }

        private void OnExecutorStarted(object sender, EventArgs e)
        {
            CurrentStep = 0;
            _executionID = Thread.CurrentThread.ManagedThreadId;
            if (Options.SteppingAI)
            {
                _isInSteppingMode = true;
                ResetEvent();
            }
        }

        [Conditional("DEBUG")]
        private void ValidateCurrentThreadID()
        {
            if (Thread.CurrentThread.ManagedThreadId != _executionID)
                throw new NotSupportedException("This execution step controller doesn't support multi-threaded execution environment.");
        }
    }
}
