using System;

namespace Renju.Infrastructure.Execution
{
    public class ReportExecutionObject : IReportExecutionStatus
    {
        public ExecutionState State { get; protected set; }

        public event EventHandler Finished;
        public event EventHandler Started;
        public event EventHandler StepFinished;

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
