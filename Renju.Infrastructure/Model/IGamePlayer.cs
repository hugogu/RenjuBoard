using System;

namespace Renju.Infrastructure.Model
{
    public interface IGamePlayer
    {
        Side Side { get; set; }

        IGameBoard<IReadOnlyBoardPoint> Board { get; set; }
    }
}
