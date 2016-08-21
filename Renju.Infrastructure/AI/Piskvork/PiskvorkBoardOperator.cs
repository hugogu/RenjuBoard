namespace Renju.Infrastructure.AI.Piskvork
{
    using System;
    using IO;
    using Microsoft.Practices.Unity;
    using Model;

    /// <summary>
    /// Protocol Specification: http://petr.lastovicka.sweb.cz/protocl2en.htm
    /// </summary>
    public class PiskvorkBoardOperator : IBoardOperator
    {
        private readonly IMessanger<string, string> _messanger;

        public PiskvorkBoardOperator([Dependency("Console")] IMessanger<string, string> messanger)
        {
            _messanger = messanger;
        }

        public void Put(PieceDrop position)
        {
            _messanger.SendAsync(String.Format("{0},{1}\r\n", position.X, position.Y));
        }

        public void ShowError(string error)
        {
            _messanger.SendAsync("ERROR " + error);
        }

        public void ShowInfo(AIInfo info)
        {
            _messanger.SendAsync(info.ToString());
        }

        public void ShowMessage(string message, bool isPublic)
        {
            if (isPublic)
                _messanger.SendAsync("MESSAGE " + message);
            else
                _messanger.SendAsync("DEBUG " + message);
        }

        public void Take(BoardPosition position)
        {
            /* Noop, no supported */
        }

        public void Thinking(BoardPosition position)
        {
            // _messanger.SendAsync("SUGGEST " + String.Format("{0},{1}", position.X, position.Y));
        }
    }
}
