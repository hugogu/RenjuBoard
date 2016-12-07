namespace Renju.Infrastructure.Protocols
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using AI;
    using Events;
    using Model;

    [ContractClassFor(typeof(IAIController))]
    public abstract class AIControllerContract : IAIController
    {
        AIInfo IAIController.AIInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<AIInfo>() != null);

                return default(AIInfo);
            }
        }

        event EventHandler<GenericEventArgs<BoardPosition>> IAIController.Dropping
        {
            add { }
            remove { }
        }

        event EventHandler<GenericEventArgs<string>> IAIController.Says
        {
            add { }
            remove { }
        }

        Task IAIController.BeginAsync()
        {
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        Task IAIController.EndAsync()
        {
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        Task IAIController.InfoAsync(GameInfo info)
        {
            Contract.Requires(info != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        Task IAIController.InitializeAsync(int boardSize)
        {
            Contract.Requires(boardSize > 0);
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        Task IAIController.LoadAsync(IEnumerable<PieceDrop> drops)
        {
            Contract.Requires(drops != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        Task IAIController.OpponentDropsAsync(BoardPosition stone)
        {
            Contract.Requires(stone != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        Task IAIController.RequestAboutAsync()
        {
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }
    }
}
