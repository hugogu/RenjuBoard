namespace Renju.Core
{
    using System.Linq;
    using Infrastructure.Execution;
    using Infrastructure.Model;
    using AI;

    public static class GameBoardUtils
    {
        public static IReadBoardState<IReadOnlyBoardPoint> With(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point)
        {
            return new GameBoardDecoration(board, point);
        }

        public static IReadOnlyBoardPoint As(this IReadOnlyBoardPoint point, Side side, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return new VirtualBoardPoint(point, side, board.DroppedPoints.Count() + 1);
        }

        public static ExecutionTimer GetExecutionTimer(this IGameBoard<IReadOnlyBoardPoint> gameBoard, IGamePlayer player)
        {
            if (player is ResolverBasedAIGamePlayer)
            {
                return (player as ResolverBasedAIGamePlayer).Resolver.ExecutionTimer;
            }
            else
            {
                return new SideExecutionReporter(gameBoard, player.Side).ExecutionTimer;
            }
        }
    }
}
