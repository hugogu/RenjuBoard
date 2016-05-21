namespace Renju.Infrastructure.AI
{
    using System;
    using System.Collections.Generic;
    using Events;
    using Model;

    public interface IBoardMonitor
    {
        event EventHandler<GenericEventArgs<int>> Initailizing;

        event EventHandler Starting;

        event EventHandler Ended;

        event EventHandler<GenericEventArgs<PieceDrop>> Dropped;

        event EventHandler<GenericEventArgs<BoardPosition>> Taken;

        event EventHandler<GenericEventArgs<IEnumerable<PieceDrop>>> Loading;
    }
}
