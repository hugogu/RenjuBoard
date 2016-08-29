namespace Renju.Infrastructure.AI
{
    using System;
    using System.Collections.Generic;
    using Events;
    using Model;
    using Protocols;

    public interface IAIController
    {
        event EventHandler<GenericEventArgs<BoardPosition>> Dropping;

        event EventHandler<GenericEventArgs<string>> Says;

        AIInfo AIInfo { get; }

        void Initialize(int boardSize);

        void Begin();

        void Info(GameInfo inf);

        void Load(IEnumerable<PieceDrop> drops);

        void OpponentDrops(BoardPosition stone);

        void End();

        void RequestAbout();
    }
}
