using System.Diagnostics;
using Microsoft.Practices.Unity.Utility;

namespace Renju.Infrastructure.Model.Extensions
{
    public static class PointExtensions
    {
        public static string GetLiternalPresentation(this Side? side)
        {
            return side == null ? "_" : (side == Side.Black ? "●" : "○");
        }

        public static PieceLine To(this IReadOnlyBoardPoint pointFrom, IReadOnlyBoardPoint pointTo, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Guard.ArgumentNotNull(pointFrom, "pointFrom");
            Guard.ArgumentNotNull(pointTo, "pointTo");
            Guard.ArgumentNotNull(board, "board");
            Debug.Assert(pointFrom != pointTo);

            return new PieceLine(board, pointFrom.Position, pointTo.Position, false);
        }
    }
}
