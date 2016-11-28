namespace Renju.Infrastructure.Model.Extensions
{
    using System.Diagnostics;
    using Microsoft.Practices.Unity.Utility;

    public static class PointExtensions
    {
        public static string GetLiternalPresentation(this Side? side)
        {
            return side == null ? "_" : (side == Side.Black ? "●" : "○");
        }

        public static PieceLine To(this IReadOnlyBoardPoint pointFrom, IReadOnlyBoardPoint pointTo, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Guard.ArgumentNotNull(pointFrom, nameof(pointFrom));
            Guard.ArgumentNotNull(pointTo, nameof(pointTo));
            Guard.ArgumentNotNull(board, nameof(board));
            Debug.Assert(pointFrom != pointTo, "from and to point must be different.");

            return new PieceLine(board, pointFrom.Position, pointTo.Position, false);
        }
    }
}
