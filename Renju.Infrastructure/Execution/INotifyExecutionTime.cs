namespace Renju.Infrastructure.Execution
{
    using System;

    public interface INotifyExecutionTime
    {
        event EventHandler Started;

        event EventHandler Finished;
    }
}
