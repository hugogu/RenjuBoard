namespace Renju.Infrastructure.Execution
{
    using System;

    public class ReportExecutionObject : DisposableModelBase, IReportExecutionStatus
    {
        public ReportExecutionObject()
        {
            ExecutionTimer = new ExecutionTimer(this);
            AutoDispose(ExecutionTimer);
        }

        public event EventHandler Finished;

        public event EventHandler Started;

        public event EventHandler StepFinished;

        public ExecutionState State { get; protected set; }

        public ExecutionTimer ExecutionTimer { get; private set; }

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
