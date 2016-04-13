using System;
using System.Reactive.Linq;

namespace Renju.Infrastructure.Execution
{
    public class ExecutionTimer : ModelBase, IDisposable
    {
        private readonly IDisposable _timingNotifier;
        private TimeSpan _executedTime = TimeSpan.FromSeconds(0);
        private DateTime? _lastExecutionStartTime;
        private TimeSpan? _pausedDuration;

        public ExecutionTimer(INotifyExecutionTime executor, int notifyIntervalInMs = 1000)
        {
            executor.Started += OnExecutorStarted;
            executor.Finished += OnExecutorFinished;
            _timingNotifier = Observable.Interval(TimeSpan.FromMilliseconds(notifyIntervalInMs)).Subscribe(_ =>
            {
                if (_lastExecutionStartTime.HasValue)
                {
                    RaisePropertyChanged(() => CurrentExecutionTime);
                    RaisePropertyChanged(() => TotalExecutionTime);
                }
            });
        }

        public TimeSpan CurrentExecutionTime
        {
            get
            {
                return _pausedDuration.HasValue ? _pausedDuration.Value :
                       (_lastExecutionStartTime.HasValue ? DateTime.Now - _lastExecutionStartTime.Value : TimeSpan.Zero);
            }
        }

        public TimeSpan TotalExecutionTime
        {
            get { return _executedTime + CurrentExecutionTime; }
        }

        public void Pause()
        {
            if (_lastExecutionStartTime.HasValue)
            {
                _pausedDuration = DateTime.Now - _lastExecutionStartTime.Value;
                _lastExecutionStartTime = null;
            }
            else
                throw new InvalidOperationException("Can't pause when it is not executing.");
        }

        public void Resume()
        {
            if (_pausedDuration.HasValue)
            {
                _lastExecutionStartTime = DateTime.Now - _pausedDuration;
                _pausedDuration = null;
            }
        }

        public virtual void Dispose()
        {
            _timingNotifier.Dispose();
        }

        private void OnExecutorFinished(object sender, EventArgs e)
        {
            if (_lastExecutionStartTime.HasValue)
            {
                _executedTime += DateTime.Now - _lastExecutionStartTime.Value;
                _lastExecutionStartTime = null;
            }
            else if (_pausedDuration.HasValue)
                throw new InvalidOperationException("Can handle finished event while paused.");
        }

        private void OnExecutorStarted(object sender, EventArgs e)
        {
            _lastExecutionStartTime = DateTime.Now;
        }
    }
}
