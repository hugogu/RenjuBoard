﻿namespace Renju.Core
{
    public interface IGameRule
    {
        string Name { get; }

        bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);

        bool? Win(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop);

        Side? NextSide(IReadBoardState<IReadOnlyBoardPoint> board);
    }
}
