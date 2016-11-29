namespace Renju.Infrastructure.Model.Extensions
{
    using System.Diagnostics.Contracts;

    public static class PointExtensions
    {
        [Pure]
        public static string GetLiternalPresentation(this Side? side)
        {
            return side == null ? "_" : (side == Side.Black ? "●" : "○");
        }

        [Pure]
        public static PieceLine To(this IReadOnlyBoardPoint pointFrom, IReadOnlyBoardPoint pointTo, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Contract.Requires(pointFrom != null);
            Contract.Requires(pointTo != null);
            Contract.Requires(board != null);
            Contract.Requires(pointFrom != pointTo, "from and to point must be different.");

            return new PieceLine(board, pointFrom.Position, pointTo.Position, false);
        }
    }
}
