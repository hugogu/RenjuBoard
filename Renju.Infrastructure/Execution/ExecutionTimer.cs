using System;
using System.Reactive.Linq;

namespace Renju.Infrastructure.Execution
{
    // TODO: Support pausing
    public class ExecutionTimer : ModelBase, IDisposable
    {
        private readonly IDisposable _timingNotifier;
        private TimeSpan _executedTime = TimeSpan.FromSeconds(0);
        private DateTime? _lastExecutionStartTime;

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
            get { return _lastExecutionStartTime.HasValue ? DateTime.Now - _lastExecutionStartTime.Value : TimeSpan.Zero; }
        }

        public TimeSpan TotalExecutionTime
        {
            get { return _executedTime + CurrentExecutionTime; }
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
        }

        private void OnExecutorStarted(object sender, EventArgs e)
        {
            _lastExecutionStartTime = DateTime.Now;
        }
    }
}
