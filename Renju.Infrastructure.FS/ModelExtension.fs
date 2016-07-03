module ModelExtension

open Renju.Infrastructure.Model
open Renju.Util
open System
open System.Linq

type BoardPosition with
    member this.IsOnBoard(board: IReadBoardState<'T>) = 
        this.X >= 0 && this.Y >= 0 &&
        this.X < board.Size && this.Y < board.Size
    member this.IsOnLineWith anotherPosition = 
        let diff = this - anotherPosition;
        diff.X = 0 || diff.Y = 0 || Math.Abs(diff.X) = Math.Abs(diff.Y)
    member this.IsDropped(board: IReadBoardState<'T>) = board.[this].Status.HasValue
    member this.IsEmptyAndWithinBoard board = 
        this.IsOnBoard board && not (this.IsDropped board)
    member this.IsOnLine(line: PieceLine) = 
        ((this.X - line.StartPosition.X) * (this.Y - line.EndPosition.Y) =
        (this.X - line.EndPosition.X) * (this.Y - line.StartPosition.Y))
    member this.IsInLine(line: PieceLine) = 
        this.X.InRang line.StartPosition.X line.EndPosition.X &&
        this.Y.InRang line.StartPosition.Y line.EndPosition.Y &&
        this.IsOnLine(line)
    member this.IsWithInLine(line: PieceLine) = 
        this.X.WithInRang line.StartPosition.X line.EndPosition.X &&
        this.Y.WithInRang line.StartPosition.Y line.EndPosition.Y &&
        this.IsOnLine(line)


type IReadBoardState<'T when 'T :> IReadOnlyBoardPoint> with
    member this.Contains(p: BoardPosition) = p.X >= 0 && p.Y >= 0 && p.X < this.Size && p.Y < this.Size
    member this.SideOfLastDrop() = this.DroppedPoints.Last().Status.Value
    member this.IterateNearbyPointsOf(point: 'T, distance: int, onLineOnly: bool) = seq {
        for x in Math.Max(0, point.Position.X - distance)..Math.Min(this.Size - 1, point.Position.X + distance) do
          for y in Math.Max(0, point.Position.Y - distance)..Math.Min(this.Size - 1, point.Position.Y + distance) do
            if x <> point.Position.X || y <> point.Position.Y then
                let position = { X = x; Y = y}
                if not onLineOnly || position.IsOnLineWith(point.Position) then
                    yield this.[position]
         }
    member this.InvalidateNearbyPointsOf point = 
        for nearbyPoint in this.IterateNearbyPointsOf point do
            nearbyPoint.RequiresReevaluateWeight <- true



type PieceLine with
    member this.Points = seq { 
        let mutable p = this.StartPosition
        while p <> this.EndPosition do
          if p.IsOnBoard(this.Board) then
            yield this.Board.[p]
          p <- p + this.Direction
        yield this.Board.[p]
     }
    member this.DroppedCount = 
        this.Points.Count(fun p -> p.Status.HasValue)
    member this.Side = this.Points |> List.ofSeq
                                   |> List.filter(fun p -> p.Status.HasValue)
                                   |> List.map(fun p -> p.Status.Value)
                                   |> List.head