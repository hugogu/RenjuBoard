namespace Renju.Infrastructure.AI
{
    using System;
    using Model;

    public interface IBoardOperator
    {
        void ShowInfo(AIInfo info);

        void Put(PieceDrop position);

        void Take(BoardPosition position);

        void Thinking(BoardPosition position);

        void ShowMessage(String message, bool isPublic);

        void ShowError(String error);
    }
}
