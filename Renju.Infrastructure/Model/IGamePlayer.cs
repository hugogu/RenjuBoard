﻿namespace Renju.Infrastructure.Model
{
    using System.Collections.Generic;
    using Protocols;

    public interface IGamePlayer
    {
        Side Side { get; set; }

        string Name { get; set; }

        string AuthorName { get;set; }

        string Country { get; set; }

        void PlayOn(IBoardMonitor board);
    }
}
