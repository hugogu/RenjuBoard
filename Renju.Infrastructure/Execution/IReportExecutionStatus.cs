using System;

namespace Renju.Infrastructure.Execution
{
    public interface IReportExecutionStatus : INotifyExecutionTime, IDisposable
    {
        ExecutionState State { get; }

        ExecutionTimer ExecutionTimer { get; }

        event EventHandler StepFinished;
    }
}
