namespace Renju.Infrastructure.Protocols.Piskvork
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
        private readonly IMessenger<string, string> _messenger;

        public PiskvorkBoardOperator([Dependency("Console")] IMessenger<string, string> messenger)
        {
            _messenger = messenger;
        }

        public void Put(PieceDrop position)
        {
            _messenger.SendAsync(String.Format("{0},{1}\r\n", position.X, position.Y));
        }

        public void ShowError(string error)
        {
            _messenger.SendAsync("ERROR " + error);
        }

        public void ShowInfo(AIInfo info)
        {
            _messenger.SendAsync(info.ToString());
        }

        public void ShowMessage(string message, bool isPublic)
        {
            if (isPublic)
                _messenger.SendAsync("MESSAGE " + message);
            else
                _messenger.SendAsync("DEBUG " + message);
        }

        public void Take(BoardPosition position)
        {
            /* Noop, no supported */
        }

        public void Thinking(BoardPosition position)
        {
            // _messenger.SendAsync("SUGGEST " + String.Format("{0},{1}", position.X, position.Y));
        }
    }
}
