namespace Renju.Infrastructure.Model.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public static class PieceLineExtensions
    {
        private static Func<IReadOnlyBoardPoint, bool> _dropped = p => p.Status.HasValue;
        private static Func<IReadOnlyBoardPoint, bool> _empty = p => p.Status == null;
        private static Func<IReadOnlyBoardPoint, bool>[] ddd = new[] { _dropped, _dropped, _dropped };
        private static Func<IReadOnlyBoardPoint, bool>[] dded = new[] { _dropped, _dropped, _empty, _dropped };
        private static Func<IReadOnlyBoardPoint, bool>[] dedd = new[] { _dropped, _empty, _dropped, _dropped };

        [Pure]
        public static int GetWeightOnBoard(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            if (line.IsClosed(board))
                return line.DroppedCount * 2 - line.Length;

            var opponentSide = board.SideOfLastDrop();
            if (opponentSide == line.Side)
            {
                return line.DroppedCount * 4 - line.Length;
            }
            else
            {
                return line.DroppedCount * 4 - line.Length + 1;
            }
        }

        [Pure]
        public static IEnumerable<PieceLine> BreakWith(this PieceLine line, BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Contract.Requires(position.IsWithInLine(line), "Position is not within line");

            var startLine = new PieceLine(board, line.StartPosition, position, line.Direction);
            if (startLine.Length > 2)
            {
                var trimedLine = (startLine - 1).TrimEnd();
                if (trimedLine != null)
                    yield return trimedLine;
            }

            var endLine = new PieceLine(board, position, line.EndPosition, line.Direction);
            if (endLine.Length > 2)
            {
                var trimedLine = (1 - endLine).TrimStart();
                if (trimedLine != null)
                    yield return trimedLine;
            }
        }

        [Pure]
        public static bool IsClosed(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            if (line.DroppedCount <= 3)
                return line.IsClosedThree(board);
            else
                return line.IsClosedFour(board);
        }

        [Pure]
        public static IEnumerable<IReadOnlyBoardPoint> GetBlockPoints(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Contract.Requires(line.DroppedCount >= 3, "line must have at least 3 drops.");
            Contract.Requires(line.StartPosition.IsDropped(board), "line start position must be dropped.");
            Contract.Requires(line.EndPosition.IsDropped(board), "line end position must be dropped.");

            if (line.DroppedCount >= 4)
            {
                return line.GetBlockPointsForFour(board);
            }
            else if (line.DroppedCount == 3)
            {
                return line.GetBlockPointsForThree(board);
            }

            return new IReadOnlyBoardPoint[0];
        }

        [Pure]
        public static IEnumerable<IReadOnlyBoardPoint> GetContinousPoints(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Contract.Requires(line.DroppedCount == 2, "line must have 2 drops.");
            Contract.Requires(line.EndPosition.IsDropped(board), "line end must be dropped.");
            Contract.Requires(line.StartPosition.IsDropped(board), "line start must be dropped.");

            return (2 + line + 2).Points.Where(p => !p.Position.IsDropped(board));
        }

        [Pure]
        public static bool FindSubLineMatch(this PieceLine line, Func<IReadOnlyBoardPoint, bool>[] pattern, out PieceLine result)
        {
            Contract.Requires(line.Length >= 3, "line must be longer than 2.");
            for (var i = 0; i < line.Length; i++)
            {
                for (var c = 0; c < pattern.Length && i + c < line.Length; c++)
                {
                    if (!pattern[c](line[i + c]))
                        break;
                    else if (c == pattern.Length - 1)
                    {
                        result = new PieceLine(line.Board, line[i].Position, line[i + c].Position);
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        [Pure]
        public static bool TryFindThreeInLine(this PieceLine line, out PieceLine result)
        {
            return line.FindSubLineMatch(ddd, out result) ||
                   line.FindSubLineMatch(dded, out result) ||
                   line.FindSubLineMatch(dedd, out result);
        }

        [Pure]
        public static bool HasOpenThree(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            PieceLine three;

            return line.TryFindThreeInLine(out three) ? !three.IsClosed(board) : false;
        }

        [Pure]
        internal static IEnumerable<IReadOnlyBoardPoint> GetBlockPointsForFour(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Contract.Requires(line.DroppedCount >= 4, "line must contains at least 4 drops");
            if (line.Length == line.DroppedCount)
            {
                var end = (line + 1).EndPosition;
                if (end.IsOnBoard(board) && !end.IsDropped(board))
                {
                    yield return board[end];
                    yield break;
                }

                var start = (1 + line).StartPosition;
                if (start.IsOnBoard(board) && !start.IsDropped(board))
                    yield return board[start];
            }
            else if (line.Length >= line.DroppedCount + 1)
            {
                foreach (var point in line.Points)
                    if (!point.Position.IsDropped(board))
                        yield return point;
            }
        }

        [Pure]
        internal static IEnumerable<IReadOnlyBoardPoint> GetBlockPointsForThree(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return (1 + line + 1).Points.Where(p => !p.Position.IsDropped(board));
        }

        [Pure]
        internal static bool IsClosedFour(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Contract.Requires(line.Length > 3, "line must be longer than 3");
            Contract.Requires(line.DroppedCount >= 4, "line must contains at least 4 drops.");
            if (line.Length == 4)
            {
                var opponentSide = Sides.Opposite(line.Side);
                var extendedline = 1 + line + 1;
                return extendedline.StartPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board) &&
                       extendedline.EndPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board);
            }

            if (line.Length == 5)
            {
                return false;
            }
            else
            {
                return !line.HasOpenThree(board);
            }
        }

        [Pure]
        internal static bool IsClosedThree(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            var opponentSide = Sides.Opposite(line.Side);
            var extendedline = 1 + line + 1;
            if (extendedline.StartPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board) ||
                extendedline.EndPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board))
                return true;

            extendedline = 1 + extendedline + 1;

            return extendedline.StartPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board) &&
                   extendedline.EndPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board);
        }
    }
}
