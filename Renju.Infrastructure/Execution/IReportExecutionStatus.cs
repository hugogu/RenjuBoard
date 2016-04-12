using System;

namespace Renju.Infrastructure.Execution
{
    public interface IReportExecutionStatus : INotifyExecutionTime
    {
        ExecutionState State { get; }

        ExecutionTimer ExecutionTimer { get; }

        event EventHandler StepFinished;
    }
}
