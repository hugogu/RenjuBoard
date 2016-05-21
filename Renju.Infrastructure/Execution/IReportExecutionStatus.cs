namespace Renju.Infrastructure.Execution
{
    using System;

    public interface IReportExecutionStatus : INotifyExecutionTime, IDisposable
    {
        event EventHandler StepFinished;

        ExecutionState State { get; }

        ExecutionTimer ExecutionTimer { get; }
    }
}
