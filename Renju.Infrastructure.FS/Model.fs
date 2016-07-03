namespace Renju.Infrastructure.Model

open Renju.Util

open System
open System.Diagnostics
open System.Collections.Generic
open System.Windows

type Side = Black = 1 | White = 2

type OperatorType = AI | Human | UndoOrRedo | Loading

[<Serializable>]
[<DebuggerDisplay("[X={X}, Y={Y}]")>]
type BoardPosition = {
    X : int;
    Y : int }
with
    override this.ToString() = String.Format("({0},{1})", this.X, this.Y)
    static member (~-) (p: BoardPosition) = { X = -p.X; Y = -p.Y }
    static member (+) (p1: BoardPosition, p2: BoardPosition) = { X = p1.X + p2.X; Y = p1.Y + p2.Y }
    static member (-) (p1: BoardPosition, p2: BoardPosition) = { X = p1.X - p2.X; Y = p1.Y - p2.Y }
    static member (*) (p: BoardPosition, length) = { X = p.X * length; Y = p.Y * length }

type DropResult = {
    HasAnySideWon : bool;
    WonSide : Nullable<Side>
    ExpectedNextSide : Nullable<Side> }
with
    static member InvalidDrop = {
        HasAnySideWon = false;
        WonSide = Nullable();
        ExpectedNextSide = Nullable()
    }
    static member Win side = {
        DropResult.InvalidDrop with
            HasAnySideWon = true;
            WonSide = side
    }
    static member NoWin side = {
        DropResult.InvalidDrop with
            ExpectedNextSide = side
    }

[<Serializable>]
[<DebuggerDisplay("{Side}: X={X}, Y={Y}")>]
type PieceDrop(x: int, y: int, side: Side) =
    new(p: BoardPosition, side: Side) = PieceDrop(p.X, p.Y, side)
    member this.X = x
    member this.Y = y
    member this.Side = side
    override this.ToString() = String.Format("({0},{1}){2}", this.X, this.Y, if this.Side = Side.Black then "●" else "○")

type PieceDropEventArgs(drop: PieceDrop, operatorType: OperatorType) =
    inherit EventArgs()
    member this.Drop = drop
    member this.OperatorType = operatorType

type IReadOnlyBoardPoint = 
    abstract member Position : BoardPosition
    abstract member Index : Nullable<int>
    abstract member Status : Nullable<Side>
    abstract member Weight : int with get, set
    abstract member RequiresReevaluateWeight : bool with get, set

[<Serializable>]
[<DebuggerVisualizer("Renju.Core.Debugging.RenjuBoardVisualizer, Renju.Core")>]
[<DebuggerDisplay("({StartPosition.X},{StartPosition.Y})->({EndPosition.X},{EndPosition.Y})")>]
type PieceLine(board: IReadBoardState<IReadOnlyBoardPoint>, start: BoardPosition, endPosition: BoardPosition) = 
    let direction = { X = GetDirection(start.X, endPosition.X); Y = GetDirection(start.Y, endPosition.Y) }
    let length = Math.Max(Math.Abs(start.X - endPosition.X), Math.Abs(start.Y - endPosition.Y)) + 1
    member this.StartPosition = start
    member this.EndPosition = endPosition
    member this.Board = board
    member this.Direction = direction
    member this.Length = length
    member this.MidelPosition = new Point(((double this.StartPosition.X) + (double this.EndPosition.X)) / 2.0, ((double this.StartPosition.Y) + (double this.EndPosition.Y)) / 2.0)
    member this.Item index = board.[start + direction * index]
    static member (-) (offset: int, x: PieceLine) = new PieceLine(x.Board, x.StartPosition + x.Direction * offset, x.EndPosition)
    static member (+) (offset: int, x: PieceLine) = new PieceLine(x.Board, x.StartPosition - x.Direction * offset, x.EndPosition)
    static member (-) (x: PieceLine, offset: int) = new PieceLine(x.Board, x.StartPosition, x.EndPosition - x.Direction * offset)
    static member (+) (x: PieceLine, offset: int) = new PieceLine(x.Board, x.StartPosition, x.EndPosition + x.Direction * offset)
    static member (+) (x: PieceLine option, y: PieceLine option) =
         match (x, y) with
         | (Some(a), Some(b)) -> 
            if a.StartPosition = b.EndPosition then
                Some(new PieceLine(a.Board, b.StartPosition, a.EndPosition))
            else if a.StartPosition = b.StartPosition then
                Some(new PieceLine(a.Board, a.EndPosition, b.EndPosition))
            else if a.EndPosition = b.StartPosition then
                Some(new PieceLine(a.Board, a.StartPosition, b.EndPosition))
            else if a.EndPosition = b.EndPosition then
                Some(new PieceLine(a.Board, a.StartPosition, b.StartPosition))
            else
                None
         | _ -> None


and IGameRule =
    abstract member Name: string
    abstract member IsOptional : bool
    abstract member IsEnabled: bool
    abstract member CanDropOn : IReadBoardState<IReadOnlyBoardPoint> * PieceDrop -> Nullable<bool>
    abstract member Win : IReadBoardState<IReadOnlyBoardPoint> * PieceDrop -> Nullable<bool>
    abstract member NextSide : IReadBoardState<IReadOnlyBoardPoint> -> Nullable<Side>

and IGameRuleEngine =
    abstract member ApplicableRules : IEnumerable<IGameRule>
    abstract member ProcessDrop : IReadBoardState<IReadOnlyBoardPoint> * PieceDrop -> DropResult
    abstract member CanDropOn : IReadBoardState<IReadOnlyBoardPoint> * PieceDrop -> bool
    abstract member IsWin : IReadBoardState<IReadOnlyBoardPoint> * PieceDrop -> Nullable<Side>

and IReadBoardState<'T when 'T :> IReadOnlyBoardPoint> = 
    [<CLIEvent>]
    abstract member PieceDropped : IEvent<EventHandler<PieceDropEventArgs>, PieceDropEventArgs>
    [<CLIEvent>]
    abstract member Taken : IEvent<EventHandler<BoardPosition>, BoardPosition>
    abstract member RuleEngine : IGameRuleEngine
    abstract member Size : int
    abstract member DropsCount : int
    abstract member VisualBoard : string
    abstract member DroppedPoints : IEnumerable<'T>
    abstract member Points : IEnumerable<'T>
    abstract member Lines : IEnumerable<PieceLine>
    abstract member Item : BoardPosition -> 'T

