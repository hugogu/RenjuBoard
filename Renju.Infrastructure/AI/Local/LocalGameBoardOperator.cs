namespace Renju.Infrastructure.AI.Local
{
    using System.Diagnostics;
    using Model;

    public class LocalGameBoardOperator : ModelBase, IBoardOperator
    {
        private readonly IGameBoard<IReadOnlyBoardPoint> _gameBoard;

        public LocalGameBoardOperator(IGameBoard<IReadOnlyBoardPoint> gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public void Put(PieceDrop position)
        {
            _gameBoard.Drop(position, OperatorType.AI);
        }

        public void Take(BoardPosition position)
        {
            _gameBoard.Take(position);
        }

        public void ShowError(string error)
        {
            Trace.TraceError(error);
        }

        public void ShowMessage(string message, bool isPublic)
        {
            if (isPublic)
                Trace.WriteLine(message);
            else
                Debug.WriteLine(message);
        }

        public void ShowInfo(AIInfo info)
        {
        }

        public void Thinking(BoardPosition position)
        {
        }
    }
}
