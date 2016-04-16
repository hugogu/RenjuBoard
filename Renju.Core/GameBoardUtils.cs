using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Renju.Infrastructure;

namespace Renju.Core
{
    public static class GameBoardUtils
    {
        public static void InvalidateNearbyPointsOf(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point)
        {
            foreach (var affectedPoint in board.IterateNearbyPointsOf(point))
            {
                affectedPoint.RequiresReevaluateWeight = true;
            }
        }

        public static IEnumerable<IReadOnlyBoardPoint> IterateNearbyPointsOf(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point, bool onLineOnly = true)
        {
            for (var x = Math.Max(0, point.Position.X - 4); x < Math.Min(board.Size, point.Position.X + 5); x++)
            {
                for (var y = Math.Max(0, point.Position.Y - 4); y < Math.Min(board.Size, point.Position.Y + 5); y++)
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

        public static bool IsOnLineWith(this BoardPosition position, BoardPosition anotherPosition)
        {
            var diff = position - anotherPosition;

            return diff.X == 0 || diff.Y == 0 || Math.Abs(diff.X) == Math.Abs(diff.Y);
        }

        public static IEnumerable<PieceLine> GetRowsOnBoard(this IReadOnlyBoardPoint point, IReadBoardState<IReadOnlyBoardPoint> board, bool includeBlank = false)
        {
            foreach (var direction in GetHalfDirections())
            {
                var line = GetRowOnBoard(point.Position, board, direction, includeBlank);
                var oppositeLine = GetRowOnBoard(point.Position, board, direction.GetOpposite(), includeBlank);
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

        public static PieceLine GetRowOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition direction, bool includeBlank)
        {
            return includeBlank ? position.GetDashRowOnBoard(board, direction) : position.GetContinuousRowOnBoard(board, direction);
        }

        public static PieceLine GetContinuousRowOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition direction)
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

        public static PieceLine GetDashRowOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition direction)
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

        public static bool IsOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < board.Size && position.Y < board.Size;
        }

        public static IEnumerable<IReadOnlyBoardPoint> GetBlockPoints(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(line.DroppedCount >= 3);
            Debug.Assert(line.StartPosition.IsDropped(board));
            Debug.Assert(line.EndPosition.IsDropped(board));

            if (line.DroppedCount == 4)
            {
                return line.GetBlockPointsForFour(board);
            }
            else if (line.DroppedCount == 3)
            {
                return line.GetBlockPointsForThree(board);
            }
            return new IReadOnlyBoardPoint[0];
        }

        internal static IEnumerable<IReadOnlyBoardPoint> GetBlockPointsForFour(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            Debug.Assert(line.DroppedCount >= 4);
            if (line.Length == line.DroppedCount)
            {
                var end = (line + 1).EndPosition;
                if (!end.IsDropped(board))
                {
                    yield return board[end];
                    yield break;
                }
                var start = (1 + line).StartPosition;
                if (!start.IsDropped(board))
                    yield return board[start];
            }
            else if (line.Length == line.DroppedCount + 1)
            {
                foreach(var point in line.Points)
                    if (!point.Position.IsDropped(board))
                        yield return point;
            }
        }

        internal static IEnumerable<IReadOnlyBoardPoint> GetBlockPointsForThree(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            foreach(var point in (1 + line + 1).Points)
                if (!point.Position.IsDropped(board))
                    yield return point;
        }

        public static IEnumerable<PieceLine> GetFours(this IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            return board.Lines.Where(l => l.DroppedCount == 4 && l.Side == side && !l.IsClosed(board)).ToList();
        }

        public static IEnumerable<PieceLine> GetThrees(this IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            return board.Lines.Where(l => l.DroppedCount == 3 && l.Side == side).ToList();
        }

        public static IEnumerable<PieceLine> GetOpenThrees(this IReadBoardState<IReadOnlyBoardPoint> board, Side side)
        {
            return board.GetThrees(side).Where(l => !l.IsClosed(board)).ToList();
        }

        public static bool IsClosed(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            if (line.DroppedCount <= 3)
                return line.IsClosedThree(board);
            else
                return line.IsClosedFour(board);
        }

        internal static bool IsClosedFour(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            if (line.Length == 4)
            {
                var opponentSide = Sides.Opposite(line.Side);
                var extendedline = 1 + line + 1;
                return extendedline.StartPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board) &&
                       extendedline.EndPosition.IsDroppedBySideOrOutOfBoard(opponentSide, board);
            }
            else if (line.Length >= 5)
                return false;
            else
                throw new ArgumentException("Line length must be greater than 3.");
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

        public static bool IsDropped(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return board[position].Status.HasValue;
        }

        public static bool IsEmptyAndWithinBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return !position.IsDropped(board) && position.IsOnBoard(board);
        }

        public static bool IsDroppedBySideOrOutOfBoard(this BoardPosition position, Side side, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return (position.IsDropped(board) && board[position].Status == side) || !position.IsOnBoard(board);
        }

        public static bool IsInLine(this BoardPosition position, PieceLine line)
        {
            return position.X.InRang(line.StartPosition.X, line.EndPosition.X) &&
                   position.Y.InRang(line.StartPosition.Y, line.EndPosition.Y) &&
                   IsOnLine(position, line);
        }

        public static bool IsWithInLine(this BoardPosition position, PieceLine line)
        {
            return (position.X.WithInRang(line.StartPosition.X, line.EndPosition.X) ||
                    position.Y.WithInRang(line.StartPosition.Y, line.EndPosition.Y)) &&
                   IsOnLine(position, line);
        }

        public static bool IsOnLine(this BoardPosition position, PieceLine line)
        {
            return (position.X - line.StartPosition.X) * (position.Y - line.EndPosition.Y) ==
                   (position.X - line.EndPosition.X) * (position.Y - line.StartPosition.Y);
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
            foreach (var newLine in point.GetRowsOnBoard(board, true))
                yield return newLine;
        }

        public static BoardPosition GetOpposite(this BoardPosition position)
        {
            return new BoardPosition(-position.X, -position.Y);
        }

        public static IEnumerable<BoardPosition> GetHalfDirections()
        {
            yield return new BoardPosition(1, 0);
            yield return new BoardPosition(1, 1);
            yield return new BoardPosition(0, 1);
            yield return new BoardPosition(-1, 1);
        }

        public static IReadBoardState<IReadOnlyBoardPoint> With(this IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint point)
        {
            return new GameBoardDecoration(board, point);
        }

        public static IReadOnlyBoardPoint As(this IReadOnlyBoardPoint point, Side side, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return new VirtualBoardPoint(point, side, board.DropsCount + 1);
        }

        public static int GetWeightOnBoard(this PieceLine line, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            if (line.DroppedCount >= 4)
            {
                if (line.Length == 5)
                    return 1000;
                return 0;
            }
            else if (line.DroppedCount == 3)
            {
                if (line.IsClosed(board))
                {
                    if (line.Length <= 5)
                        return 40;
                    else
                        return 0;
                }
                else
                {
                    if (line.Length == 4)
                        return 249;
                    if (line.Length == 5)
                        return 200;
                    return 0;
                }
            }
            else if (line.DroppedCount == 2)
            {
                if (line.Length == 3)
                {
                    return line.IsClosed(board) ? 5 : 55;
                }
                if (line.Length == 4)
                {
                    return line.IsClosed(board) ? 4 : 50;
                }
                if (line.Length == 5)
                {
                    return line.IsClosed(board) ? 1 : 10;
                }
            }
            else if (line.DroppedCount == 1)
            {
                return 6 - line.Length;
            }
            return 0;
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
