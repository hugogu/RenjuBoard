using System;

namespace Renju.Infrastructure.Execution
{
    public interface INotifyExecutionTime
    {
        event EventHandler Started;

        event EventHandler Finished;
    }
}
