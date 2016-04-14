﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static IEnumerable<PieceLine> GetLinesOnBoard(this IReadOnlyBoardPoint point, IReadBoardState<IReadOnlyBoardPoint> board, bool includeBlank = false)
        {
            foreach (var direction in GetHalfDirections())
            {
                var line = GetLineOnBoard(point.Position, board, direction, includeBlank);
                var oppositeLine = GetLineOnBoard(point.Position, board, direction.GetOpposite(), includeBlank);
                var combinedLine = line + oppositeLine;
                if (combinedLine != null)
                {
                    yield return combinedLine;
                    continue;
                }
                if (line != null)
                    yield return line;
                if (oppositeLine != null)
                    yield return oppositeLine;
            }
        }

        public static PieceLine GetLineOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition direction, bool includeBlank)
        {
            return includeBlank ? position.GetDashLineOnBoard(board, direction) : position.GetContinuousLineOnBoard(board, direction);
        }

        public static PieceLine GetContinuousLineOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition direction)
        {
            if (!board.IsDropped(position))
                throw new InvalidOperationException("ContinousLine can't start with blank.");

            var endPosition = position + direction;
            while (endPosition.IsOnBoard(board) && board.IsDropped(endPosition) && board[endPosition].Status.Value == board[position].Status.Value)
            {
                endPosition += direction;
            }
            endPosition -= direction;

            return Equals(position, endPosition) ? null : new PieceLine(board, position, endPosition);
        }

        public static PieceLine GetDashLineOnBoard(this BoardPosition position, IReadBoardState<IReadOnlyBoardPoint> board, BoardPosition direction)
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

        public static IEnumerable<PieceLine> BreakWith(this PieceLine line, BoardPosition position)
        {
            Debug.Assert(position.IsWithInLine(line), String.Format("Position {0} is not within line {1}", position, line));

            var startLine = new PieceLine(line.Board, line.StartPosition, position);
            if (startLine.Length > 2)
                yield return startLine - 1;
            var endLine = new PieceLine(line.Board, line.EndPosition, position);
            if (endLine.Length > 2)
                yield return endLine - 1;
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
                    foreach (var breakLine in line.BreakWith(point.Position))
                        yield return breakLine;
                else
                    yield return line;
            }
            foreach (var newLine in point.GetLinesOnBoard(board, true))
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
                if (line.IsClosed)
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
                    return line.IsClosed ? 5 : 55;
                }
                if (line.Length == 4)
                {
                    return line.IsClosed ? 4 : 50;
                }
                if (line.Length == 5)
                {
                    return line.IsClosed ? 1 : 10;
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

            if (!board.IsDropped(nextPosition))
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
