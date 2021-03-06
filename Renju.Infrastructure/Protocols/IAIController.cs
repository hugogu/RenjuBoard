﻿namespace Renju.Infrastructure.AI
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

        Task InitializeAsync(int boardSize);

        Task BeginAsync();

        Task InfoAsync(GameInfo inf);

        Task LoadAsync(IEnumerable<PieceDrop> drops);

        Task OpponentDropsAsync(BoardPosition stone);

        Task EndAsync();

        Task RequestAboutAsync();
    }
}
