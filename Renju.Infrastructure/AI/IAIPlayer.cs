namespace Renju.Infrastructure.AI
{
    using System;
    using System.Collections.Generic;
    using Events;
    using Model;
    using Protocols;

    public interface IAIPlayer
    {
        event EventHandler<GenericEventArgs<BoardPosition>> Dropping;

        event EventHandler<GenericEventArgs<AIInfo>> Introducing;

        event EventHandler<GenericEventArgs<String>> Says;

        void Initialize(int boardSize);

        void Begin();

        void Info(GameInfo inf);

        void Load(IEnumerable<PieceDrop> drops);

        void OpponentDrops(PieceDrop stone);

        void End();

        void RequestAbout();
    }
}
