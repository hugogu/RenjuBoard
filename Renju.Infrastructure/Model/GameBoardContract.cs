namespace Renju.Infrastructure.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Events;

    [ContractClassFor(typeof(IReadBoardState<>))]
    public abstract class ReadGameBoardContract<T> : IReadBoardState<T>
        where T : IReadOnlyBoardPoint
    {
        T IReadBoardState<T>.this[BoardPosition position]
        {
            get
            {
                Contract.Requires(position != null);
                Contract.Ensures(Contract.Result<T>() != null);

                return default(T);
            }
        }

        IEnumerable<T> IReadBoardState<T>.DroppedPoints
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

                return default(IEnumerable<T>);
            }
        }

        IEnumerable<PieceLine> IReadBoardState<T>.Lines
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<PieceLine>>() != null);

                return default(IEnumerable<PieceLine>);
            }
        }

        IEnumerable<T> IReadBoardState<T>.Points
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

                return default(IEnumerable<T>);
            }
        }

        IGameRuleEngine IReadBoardState<T>.RuleEngine
        {
            get
            {
                Contract.Ensures(Contract.Result<IGameRuleEngine>() != null);

                return default(IGameRuleEngine);
            }
        }

        int IReadBoardState<T>.Size
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);

                return default(int);
            }
        }

        event EventHandler<GenericEventArgs<NewGameOptions>> IReadBoardState<T>.Begin
        {
            add { }
            remove { }
        }

        event EventHandler<PieceDropEventArgs> IReadBoardState<T>.PieceDropped
        {
            add { }
            remove { }
        }

        event EventHandler<BoardPosition> IReadBoardState<T>.Taken
        {
            add { }
            remove { }
        }
    }

    [ContractClassFor(typeof(IGameBoard<>))]
    public abstract class GameBoardContract<T> : IGameBoard<T>
        where T : IReadOnlyBoardPoint
    {
        public abstract T this[BoardPosition position] { get; }

        public abstract IEnumerable<T> DroppedPoints { get; }
        public abstract IEnumerable<PieceLine> Lines { get; }
        public abstract IEnumerable<T> Points { get; }
        public abstract IGameRuleEngine RuleEngine { get; }
        public abstract int Size { get; }

        Side? IGameBoard<T>.ExpectedNextTurn
        {
            get
            {
                return default(Side?);
            }
        }

        public abstract event EventHandler<GenericEventArgs<NewGameOptions>> Begin;
        public abstract event EventHandler<PieceDropEventArgs> PieceDropped;
        public abstract event EventHandler<BoardPosition> Taken;

        void IGameBoard<T>.BeginGame()
        {
        }

        DropResult IGameBoard<T>.Drop(BoardPosition position, OperatorType operatorType)
        {
            Contract.Requires(position != null);
            Contract.Ensures(Contract.Result<DropResult>() != null);

            return default(DropResult);
        }

        void IGameBoard<T>.Take(BoardPosition position)
        {
            Contract.Requires(position != null);
            Contract.Requires(DroppedPoints.Any());
        }
    }
}
