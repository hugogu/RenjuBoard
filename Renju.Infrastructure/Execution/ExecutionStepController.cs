namespace Renju.Infrastructure.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.Practices.Unity;
    using Model;

    public class ExecutionStepController : DisposableModelBase, IStepController
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(true);
        private readonly IEnumerable<IReportExecutionStatus> _executors;
        private bool _isInSteppingMode = false;
        private int _executionID;
        private int _allowedSteps;

        public ExecutionStepController([Dependency] IReportExecutionStatus[] executors)
        {
            Debug.Assert(executors != null);
            _executors = executors;
            foreach (var executor in executors)
            {
                executor.Started += OnExecutorStarted;
                executor.Finished += OnExecutorFinished;
                executor.StepFinished += OnExecutorFinishedStep;
                AutoCallOnDisposing(() =>
                {
                    executor.Started -= OnExecutorStarted;
                    executor.Finished -= OnExecutorFinished;
                    executor.StepFinished -= OnExecutorFinishedStep;
                });
            }

            AutoDispose(_event);
        }

        public int CurrentStep { get; private set; }

        public bool IsPaused
        {
            get { return !_event.WaitOne(0); }
        }

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
            foreach(var executor in _executors)
                executor.ExecutionTimer.Resume();
            _event.Set();
            OnPropertyChanged(() => IsPaused);
        }

        protected virtual void ResetEvent()
        {
            _event.Reset();
            foreach(var executor in _executors)
                executor.ExecutionTimer.Pause();
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
            Debug.Assert(Thread.CurrentThread.ManagedThreadId == _executionID,
                "This execution step controller doesn't support multi-threaded execution environment.");
        }
    }
}
