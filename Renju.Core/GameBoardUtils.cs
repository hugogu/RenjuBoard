using Renju.Infrastructure.Model;

namespace Renju.Core
{
    public static class GameBoardUtils
    {
        public static IReadBoardState<IReadOnlyBoardPoint> With(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point)
        {
            return new GameBoardDecoration(board, point);
        }

        public static IReadOnlyBoardPoint As(this IReadOnlyBoardPoint point, Side side, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return new VirtualBoardPoint(point, side, board.DropsCount + 1);
        }
    }
}
