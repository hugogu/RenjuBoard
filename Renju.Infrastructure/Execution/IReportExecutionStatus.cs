using System;

namespace Renju.Infrastructure.Execution
{
    public interface IReportExecutionStatus
    {
        ExecutionState State { get; }

        event EventHandler Started;

        event EventHandler StepFinished;

        event EventHandler Finished;
    }
}
