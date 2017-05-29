namespace Renju.Infrastructure.Execution
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Linq;

    public class ExecutionTimer : DisposableModelBase
    {
        private TimeSpan _executedTime = TimeSpan.FromSeconds(0);
        private DateTime? _lastExecutionStartTime;
        private TimeSpan? _pausedDuration;

        public ExecutionTimer(INotifyExecutionTime executor, int notifyIntervalInMs = 1000)
        {
            executor.Started += OnExecutorStarted;
            executor.Finished += OnExecutorFinished;
            AutoDispose(Observable
                .Interval(TimeSpan.FromMilliseconds(notifyIntervalInMs))
                .Where(_ => IsExecutorRunning)
                .Subscribe(_ =>
            {
                OnPropertyChanged(() => CurrentExecutionTime);
                OnPropertyChanged(() => TotalExecutionTime);
            }));
        }

        public TimeSpan CurrentExecutionTime
        {
            get
            {
                return _pausedDuration.HasValue ? _pausedDuration.Value :
                       (IsExecutorRunning ? DateTime.Now - _lastExecutionStartTime.Value : TimeSpan.Zero);
            }
        }

        public TimeSpan TotalExecutionTime
        {
            get { return _executedTime + CurrentExecutionTime; }
        }

        public void Pause()
        {
            Debug.Assert(IsExecutorRunning, "Can't pause when it is not executing.");
            _pausedDuration = DateTime.Now - _lastExecutionStartTime.Value;
            _lastExecutionStartTime = null;
        }

        public void Resume()
        {
            if (_pausedDuration.HasValue)
            {
                _lastExecutionStartTime = DateTime.Now - _pausedDuration;
                _pausedDuration = null;
            }
        }

        protected bool IsExecutorRunning
        {
            get { return _lastExecutionStartTime.HasValue; }
        }

        private void OnExecutorFinished(object sender, EventArgs e)
        {
            Debug.Assert(IsExecutorRunning || !_pausedDuration.HasValue, "Can't handle finished event while paused.");

            if (IsExecutorRunning)
            {
                _executedTime += DateTime.Now - _lastExecutionStartTime.Value;
                _lastExecutionStartTime = null;
            }
        }

        private void OnExecutorStarted(object sender, EventArgs e)
        {
            _lastExecutionStartTime = DateTime.Now;
        }
    }
}
