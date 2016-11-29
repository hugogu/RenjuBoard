namespace Renju.Infrastructure.AI
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using Execution;
    using Model;

    [ContractClassFor(typeof(IDropResolver))]
    public abstract class DropResolverContract : IDropResolver
    {
        CancellationToken IDropResolver.CancelTaken
        {
            get
            {
                Contract.Ensures(Contract.Result<CancellationToken>() != null);
                return default(CancellationToken);
            }
            set
            {
                Contract.Requires(value != null);
            }
        }

        ExecutionTimer IReportExecutionStatus.ExecutionTimer { get; }

        ExecutionState IReportExecutionStatus.State { get; }

        event EventHandler INotifyExecutionTime.Finished
        {
            add { }
            remove { }
        }

        event EventHandler INotifyExecutionTime.Started
        {
            add { }
            remove { }
        }

        event EventHandler IReportExecutionStatus.StepFinished
        {
            add { }
            remove { }
        }

        void IDisposable.Dispose()
        {
        }

        Task<IReadOnlyBoardPoint> IDropResolver.ResolveAsync(IGameBoard<IReadOnlyBoardPoint> board, Side side)
        {
            Contract.Requires(board != null);
            Contract.Ensures(Contract.Result<Task<IReadOnlyBoardPoint>>() != null);

            return default(Task<IReadOnlyBoardPoint>);
        }
    }
}
