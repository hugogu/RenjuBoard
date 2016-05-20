using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Renju.Infrastructure.Model.Extensions
{
    public static class PieceLineExtensions
    {
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

        public static IEnumerable<PieceLine> BreakWith(this PieceLine line, BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(position.IsWithInLine(line), String.Format("Position {0} is not within line {1}", position, line));

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

        public static bool IsClosed(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            if (line.DroppedCount <= 3)
                return line.IsClosedThree(board);
            else
                return line.IsClosedFour(board);
        }

        public static IEnumerable<IReadOnlyBoardPoint> GetBlockPoints(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(line.DroppedCount >= 3);
            Debug.Assert(line.StartPosition.IsDropped(board));
            Debug.Assert(line.EndPosition.IsDropped(board));

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

        public static IEnumerable<IReadOnlyBoardPoint> GetContinousPoints(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(line.DroppedCount == 2);
            Debug.Assert(line.EndPosition.IsDropped(board));
            Debug.Assert(line.StartPosition.IsDropped(board));

            return (2 + line + 2).Points.Where(p => !p.Position.IsDropped(board));
        }

        public static Func<IReadOnlyBoardPoint, bool> Dropped = p => p.Status.HasValue;
        public static Func<IReadOnlyBoardPoint, bool> Empty = p => p.Status == null;
        public static Func<IReadOnlyBoardPoint, bool>[] DDD = new[] { Dropped, Dropped, Dropped };
        public static Func<IReadOnlyBoardPoint, bool>[] DDED = new[] { Dropped, Dropped, Empty, Dropped };
        public static Func<IReadOnlyBoardPoint, bool>[] DEDD = new[] { Dropped, Empty, Dropped, Dropped };

        public static bool FindSubLineMatch(this PieceLine line, Func<IReadOnlyBoardPoint, bool>[] pattern, out PieceLine result)
        {
            Debug.Assert(line.Length >= 3);
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

        public static bool TryFindThreeInLine(this PieceLine line, out PieceLine result)
        {
            return line.FindSubLineMatch(DDD, out result) ||
                   line.FindSubLineMatch(DDED, out result) ||
                   line.FindSubLineMatch(DEDD, out result);
        }

        public static bool HasOpenThree(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            PieceLine three;
            if (line.TryFindThreeInLine(out three))
            {
                return !three.IsClosed(board);
            }
            return false;
        }

        internal static IEnumerable<IReadOnlyBoardPoint> GetBlockPointsForFour(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(line.DroppedCount >= 4);
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

        internal static IEnumerable<IReadOnlyBoardPoint> GetBlockPointsForThree(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return (1 + line + 1).Points.Where(p => !p.Position.IsDropped(board));
        }

        internal static bool IsClosedFour(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(line.Length > 3);
            Debug.Assert(line.DroppedCount >= 4);
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
