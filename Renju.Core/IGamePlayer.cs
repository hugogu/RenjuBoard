using System;

namespace Renju.Core
{
    public interface IGamePlayer
    {
        Side Side { get; set; }

        IGameBoard<IReadOnlyBoardPoint> Board { get; set; }
    }
}
