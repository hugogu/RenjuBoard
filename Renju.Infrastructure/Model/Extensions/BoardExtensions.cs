namespace Renju.Infrastructure.Model.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using ReadOnlyBoard = IReadBoardState<IReadOnlyBoardPoint>;

    public static class BoardExtensions
    {
        public static IEnumerable<IReadOnlyBoardPoint> InvalidateNearbyPointsOf(this ReadOnlyBoard board, IReadOnlyBoardPoint point)
        {
            foreach (var affectedPoint in board.IterateNearbyPointsOf(point))
            {
                if (affectedPoint.Status == null)
                {
                    affectedPoint.RequiresReevaluateWeight = true;
                    yield return affectedPoint;
                }
            }
        }

        [Pure]
        public static Side SideOfLastDrop(this ReadOnlyBoard board)
        {
            Debug.Assert(board.DroppedPoints.Any(), "board should have some drops by now.");

            return board.DroppedPoints.Last().Status.Value;
        }

        [Pure]
        public static string GetLiternalPresentation<TPoint>(this IReadBoardState<TPoint> board)
            where TPoint : IReadOnlyBoardPoint
        {
            return String.Join(
                Environment.NewLine,
                board.Points.GroupBy(point => point.Position.Y)
                            .Select(row => String.Join("_", row.Select(p => p.Status.GetLiternalPresentation()))));
        }

        [Pure]
        public static IEnumerable<LineSegment> GetAllLineAroundPoint(this ReadOnlyBoard board, IReadOnlyBoardPoint point, int distance)
        {
            foreach(var direction in GetHalfDirections())
            {
                var endPoint = point.Position.MoveAlone(direction, distance);
                var startPoint = point.Position.MoveAlone(-direction, distance);

                yield return new LineSegment(board, startPoint, endPoint);
            }
        }

        [Pure]
        public static IEnumerable<IReadOnlyBoardPoint> IterateNearbyPointsOf(this ReadOnlyBoard board, IReadOnlyBoardPoint point, int distance = 4, bool onLineOnly = true)
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

        [Pure]
        public static IEnumerable<BoardPosition> GetHalfDirections()
        {
            yield return new BoardPosition(1, 0);
            yield return new BoardPosition(1, 1);
            yield return new BoardPosition(0, 1);
            yield return new BoardPosition(-1, 1);
        }

        [Pure]
        public static PieceLine GetRowOnBoard(this ReadOnlyBoard board, BoardPosition position, BoardPosition direction, bool includeBlank)
        {
            return includeBlank ? board.GetDashRowOnBoard(position, direction) : board.GetContinuousRowOnBoard(position, direction);
        }

        [Pure]
        public static PieceLine GetContinuousRowOnBoard(this ReadOnlyBoard board, BoardPosition position, BoardPosition direction)
        {
            Debug.Assert(position.IsDropped(board), "ContinousLine can't start with blank.");

            var endPosition = position + direction;
            while (endPosition.IsOnBoard(board) && endPosition.IsDropped(board) && board[endPosition].Status.Value == board[position].Status.Value)
            {
                endPosition += direction;
            }

            endPosition -= direction;

            return Equals(position, endPosition) ? null : new PieceLine(board, position, endPosition);
        }

        [Pure]
        public static PieceLine GetDashRowOnBoard(this ReadOnlyBoard board, BoardPosition position, BoardPosition direction)
        {
            var firstState = board[position].Status;
            var endPosition = position;
            while (endPosition.CanMoveAlone(board, direction, ref firstState))
            {
                endPosition += direction;
            }

            return (endPosition.Equals(position) || firstState == null) ?
                    null : new PieceLine(board, position, endPosition).TrimEnd();
        }

        [Pure]
        public static IEnumerable<PieceLine> GetRowsFromPoint(this ReadOnlyBoard board, IReadOnlyBoardPoint point, bool includeBlank = false)
        {
            foreach (var direction in GetHalfDirections())
            {
                var line = board.GetRowOnBoard(point.Position, direction, includeBlank);
                var oppositeLine = board.GetRowOnBoard(point.Position, -direction, includeBlank);
                if (line != null && oppositeLine != null && line.Side == oppositeLine.Side)
                {
                    var combinedLine = line + oppositeLine;
                    Debug.Assert(combinedLine.StartPosition.IsDropped(board), "combined Line must be dropped on start position.");
                    Debug.Assert(combinedLine.EndPosition.IsDropped(board), "combined Line must be dropped on end position.");
                    yield return combinedLine;
                    continue;
                }

                if (line != null)
                {
                    Debug.Assert(line.EndPosition.IsDropped(board), "line must be dropped on start position.");
                    yield return line;
                }

                if (oppositeLine != null)
                {
                    Debug.Assert(oppositeLine.EndPosition.IsDropped(board), "opposite Line must be dropped on start position.");
                    yield return oppositeLine;
                }
            }
        }

        [Pure]
        public static IEnumerable<PieceLine> FindAllLinesOnBoardWithoutPoint(this ReadOnlyBoard board, IReadOnlyBoardPoint point)
        {
            Debug.Assert(point.Status == null, "point mustn't been dropped.");

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

        [Pure]
        public static IEnumerable<PieceLine> FindAllLinesOnBoardWithNewPoint(this ReadOnlyBoard board, IReadOnlyBoardPoint point)
        {
            Debug.Assert(point.Status.HasValue, "point must be dropped,");

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

        [Pure]
        public static IEnumerable<PieceLine> GetLines(this ReadOnlyBoard board, Func<int, bool> dropsCount, Side? side = null, bool openOnly = false)
        {
            return board.Lines.Where(l => dropsCount(l.DroppedCount) &&
                                   (openOnly && !l.IsClosed(board)) &&
                                   (side.HasValue && side == l.Side));
        }

        [Pure]
        private static bool CanMoveAlone(this BoardPosition position, ReadOnlyBoard board, BoardPosition direction, ref Side? pickedSide)
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
