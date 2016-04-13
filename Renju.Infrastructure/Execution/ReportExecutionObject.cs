using System;

namespace Renju.Infrastructure.Execution
{
    public class ReportExecutionObject : DisposableModelBase, IReportExecutionStatus
    {
        public ReportExecutionObject()
        {
            ExecutionTimer = new ExecutionTimer(this);
            AutoDispose(ExecutionTimer);
        }

        public virtual ExecutionState State { get; protected set; }

        public virtual ExecutionTimer ExecutionTimer { get; private set; }

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
    }
}
