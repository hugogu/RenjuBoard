namespace Renju.Infrastructure.Model.Extensions
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    public static class PointExtensions
    {
        [Pure]
        public static string GetLiternalPresentation(this Side? side)
        {
            return side == null ? "_" : (side == Side.Black ? "●" : "○");
        }

        [Pure]
        public static string GetPatterOnSide(this IReadOnlyBoardPoint point, Side side)
        {
            return point.Status == null ? "_" : (point.Status == side ? "+" : "-");
        }

        [Pure]
        public static PieceLine To(this IReadOnlyBoardPoint pointFrom, IReadOnlyBoardPoint pointTo, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(pointFrom != null);
            Debug.Assert(pointTo != null);
            Debug.Assert(board != null);
            Debug.Assert(pointFrom != pointTo, "from and to point must be different.");

            return new PieceLine(board, pointFrom.Position, pointTo.Position, false);
        }
    }
}
