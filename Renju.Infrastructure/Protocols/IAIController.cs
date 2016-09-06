namespace Renju.Infrastructure.AI
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Events;
    using Model;
    using Protocols;

    public interface IAIController
    {
        event EventHandler<GenericEventArgs<BoardPosition>> Dropping;

        event EventHandler<GenericEventArgs<string>> Says;

        AIInfo AIInfo { get; }

        Task Initialize(int boardSize);

        Task Begin();

        Task Info(GameInfo inf);

        Task Load(IEnumerable<PieceDrop> drops);

        Task OpponentDrops(BoardPosition stone);

        Task End();

        Task RequestAbout();
    }
}
