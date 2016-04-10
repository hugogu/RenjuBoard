using System;
using System.Diagnostics;
using System.Threading;

namespace Renju.Infrastructure.Execution
{
    public class ExecutionStepController : ModelBase, IStepController, IDisposable
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(true);
        private bool _isInSteppingMode = false;
        private int _executionID;
        private int _allowedSteps;

        public ExecutionStepController(IReportExecutionStatus executor)
        {
            executor.Started += OnExecutorStarted;
            executor.Finished += OnExecutorFinished;
            executor.StepFinished += OnExecutorFinishedStep;
        }

        public int CurrentStep { get; private set; }

        public bool IsPaused { get { return !_event.WaitOne(0); } }

        public bool PauseOnStart { get; set; }

        public void Continue()
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

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _event.Close();
            }
        }

        protected virtual void SetEvent()
        {
            _event.Set();
            RaisePropertyChanged(() => IsPaused);
        }

        protected virtual void ResetEvent()
        {
            _event.Reset();
            RaisePropertyChanged(() => IsPaused);
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
            if (PauseOnStart)
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
