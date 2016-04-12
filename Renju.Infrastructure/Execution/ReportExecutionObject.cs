using System;

namespace Renju.Infrastructure.Execution
{
    public class ReportExecutionObject : IReportExecutionStatus, IDisposable
    {
        public ReportExecutionObject()
        {
            ExecutionTimer = new ExecutionTimer(this);
        }

        public virtual ExecutionState State { get; protected set; }

        public virtual ExecutionTimer ExecutionTimer { get; private set; }

        public event EventHandler Finished;
        public event EventHandler Started;
        public event EventHandler StepFinished;

        public virtual void Dispose()
        {
            ExecutionTimer.Dispose();
        }

        protected virtual void RaiseFinishedEvent()
        {
            State = ExecutionState.NotStarted;
            RaiseEvent(Finished);
        }

        protected virtual void RaiseStartedEvent()
        {
            State = ExecutionState.Executing;
            RaiseEvent(Started);
        }

        protected virtual void RaiseStepFinishedEvent()
        {
            State = ExecutionState.AwaitExecuteSignal;
            RaiseEvent(StepFinished);
        }

        protected void RaiseEvent(EventHandler handler)
        {
            var temp = handler;
            if (temp != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
