using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Renju.Infrastructure.Model.Extensions
{
    public static class BoardExtensions
    {
        public static void InvalidateNearbyPointsOf(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point)
        {
            foreach (var affectedPoint in board.IterateNearbyPointsOf(point))
            {
                affectedPoint.RequiresReevaluateWeight = true;
            }
        }

        public static Side SideOfLastDrop(this IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(board.DroppedPoints.Any());

            return board.DroppedPoints.Last().Status.Value;
        }

        public static string GetLiternalPresentation<TPoint>(this IReadBoardState<TPoint> board)
            where TPoint : IReadOnlyBoardPoint
        {
            return String.Join(Environment.NewLine,
                               board.Points.GroupBy(point => point.Position.Y)
                                           .Select(row => String.Join("_", row.Select(p => p.Status.GetLiternalPresentation()))));
        }

        public static IEnumerable<IReadOnlyBoardPoint> IterateNearbyPointsOf(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point, int distance = 4, bool onLineOnly = true)
        {
            for (var x = Math.Max(0, point.Position.X - distance); x <= Math.Min(board.Size - 1, point.Position.X + distance); x++)
            {
                for (var y = Math.Max(0, point.Position.Y - distance); y <= Math.Min(board.Size - 1, point.Position.Y + distance); y++)
                {
                    if (x != point.Position.X || y != point.Position.Y)
                    {
                        var position = new BoardPosition(x, y);
                        if (!onLineOnly || position.IsOnLineWith(point.Position))
                            yield return board[position];
                    }
                }
            }
        }

        public static IEnumerable<BoardPosition> GetHalfDirections()
        {
            yield return new BoardPosition(1, 0);
            yield return new BoardPosition(1, 1);
            yield return new BoardPosition(0, 1);
            yield return new BoardPosition(-1, 1);
        }

        public static PieceLine GetRowOnBoard(this IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition position, BoardPosition direction, bool includeBlank)
        {
            return includeBlank ? board.GetDashRowOnBoard(position, direction) : board.GetContinuousRowOnBoard(position, direction);
        }

        public static PieceLine GetContinuousRowOnBoard(this IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition position, BoardPosition direction)
        {
            if (!position.IsDropped(board))
                throw new InvalidOperationException("ContinousLine can't start with blank.");

            var endPosition = position + direction;
            while (endPosition.IsOnBoard(board) && endPosition.IsDropped(board) && board[endPosition].Status.Value == board[position].Status.Value)
            {
                endPosition += direction;
            }
            endPosition -= direction;

            return Equals(position, endPosition) ? null : new PieceLine(board, position, endPosition);
        }

        public static PieceLine GetDashRowOnBoard(this IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition position, BoardPosition direction)
        {
            var firstState = board[position].Status;
            var endPosition = position;
            while (endPosition.CanMoveAlone(board, direction, ref firstState))
            {
                endPosition += direction;
            }

            return (Equals(endPosition, position) || firstState == null) ?
                    null : new PieceLine(board, position, endPosition).TrimEnd();
        }

        public static IEnumerable<PieceLine> GetRowsFromPoint(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point, bool includeBlank = false)
        {
            foreach (var direction in GetHalfDirections())
            {
                var line = board.GetRowOnBoard(point.Position, direction, includeBlank);
                var oppositeLine = board.GetRowOnBoard(point.Position, -direction, includeBlank);
                var combinedLine = line + oppositeLine;
                if (combinedLine != null)
                {
                    Debug.Assert(combinedLine.StartPosition.IsDropped(board));
                    Debug.Assert(combinedLine.EndPosition.IsDropped(board));
                    yield return combinedLine;
                    continue;
                }
                if (line != null)
                {
                    Debug.Assert(line.EndPosition.IsDropped(board));
                    yield return line;
                }
                if (oppositeLine != null)
                {
                    Debug.Assert(oppositeLine.EndPosition.IsDropped(board));
                    yield return oppositeLine;
                }
            }
        }

        public static IEnumerable<PieceLine> FindAllLinesOnBoardWithoutPoint(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point)
        {
            Debug.Assert(point.Status == null);

            foreach (var line in board.Lines)
            {
                if (point.Position.IsOnLine(line))
                {
                    var trimLine = line.Trim();
                    if (trimLine != null)
                        yield return trimLine;
                }
                else
                    yield return line;
            }
            foreach (var jointLine in board.GetRowsFromPoint(point, true))
                if (jointLine.StartPosition.IsDropped(board) && jointLine.EndPosition.IsDropped(board))
                    yield return jointLine;
        }

        public static IEnumerable<PieceLine> FindAllLinesOnBoardWithNewPoint(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point)
        {
            Debug.Assert(point.Status.HasValue);

            foreach (var line in board.Lines)
            {
                if (point.Status.Value == line.Side)
                    if (point.Position.IsOnLine(line))
                        continue;
                    else
                        yield return line;
                else if (point.Position.IsWithInLine(line))
                    foreach (var breakLine in line.BreakWith(point.Position, board))
                        yield return breakLine;
                else
                    yield return line;
            }
            foreach (var newLine in board.GetRowsFromPoint(point, true))
                yield return newLine;
        }

        public static IEnumerable<PieceLine> GetLines(this IReadBoardState<IReadOnlyBoardPoint> board, Func<int, bool> dropsCount, Side? side = null, bool openOnly = false)
        {
            return board.Lines.Where(l => dropsCount(l.DroppedCount) &&
                                   (openOnly && !l.IsClosed(board)) &&
                                   (side.HasValue && side == l.Side));
        }

        private static bool CanMoveAlone(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition direction, ref Side? pickedSide)
        {
            var nextPosition = position + direction;
            if (!nextPosition.IsOnBoard(board))
                return false;

            if (!nextPosition.IsDropped(board))
                return true;

            if (pickedSide == null)
            {
                pickedSide = board[nextPosition].Status.Value;
                return true;
            }

            return pickedSide.Value == board[nextPosition].Status.Value;
        }
    }
}
