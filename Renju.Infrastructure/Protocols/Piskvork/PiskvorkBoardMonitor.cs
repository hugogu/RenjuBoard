namespace Renju.Infrastructure.Protocols.Piskvork
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Events;
    using IO;
    using Microsoft.Practices.Unity;
    using Model;

    public class PiskvorkBoardMonitor : ModelBase, IBoardMonitor
    {
        private readonly IMessenger<string, string> _messenger;
        private IList<string> _loadingDrops = null;

        public PiskvorkBoardMonitor([Dependency("Console")] IMessenger<string, string> messenger)
        {
            _messenger = messenger;
            _messenger.MessageReceived += OnMessangerMessageReceived;
        }

        public event EventHandler<GenericEventArgs<BoardPosition>> Dropped;

        public event EventHandler Ended;

        public event EventHandler<GenericEventArgs<int>> Initailizing;

        public event EventHandler<GenericEventArgs<IEnumerable<PieceDrop>>> Loading;

        public event EventHandler Starting;

        public event EventHandler<GenericEventArgs<BoardPosition>> Taken;

        public event EventHandler AboutRequested;

        private void ReplyCommand(Action handler)
        {
            try
            {
                handler();
                _messenger.SendAsync("OK");
            }
            catch (Exception ex)
            {
                _messenger.SendAsync("ERROR message - " + ex.Message);
            }
        }

        private void OnMessangerMessageReceived(object sender, GenericEventArgs<string> e)
        {
            var command = e.Message;
            if (command == null)
                return;

            if (command.StartsWith("START", StringComparison.Ordinal))
            {
                ReplyCommand(() => HandleStartCommand(command));
            }
            else if (command.StartsWith("BEGIN", StringComparison.Ordinal))
            {
                RaiseEvent(Starting);
            }
            else if (command.StartsWith("TURN", StringComparison.Ordinal))
            {
                HandleTurnCommand(command);
            }
            else if (command.StartsWith("INFO", StringComparison.Ordinal))
            {
                Trace.TraceInformation(command.Substring(4));
            }
            else if (command.StartsWith("BOARD", StringComparison.Ordinal))
            {
                _loadingDrops = new List<string>();
            }
            else if (command.StartsWith("DONE", StringComparison.Ordinal))
            {
                var drops = (from drop in _loadingDrops
                             let axis = drop.Split(',')
                             select new
                             {
                                 Position = drop.AsBoardPosition(),
                                 N = Convert.ToInt32(axis[2])
                             }).ToList();
                Func<int, Side> sideConverter = n => n == drops[0].N ? Side.Black : Side.White;
                RaiseEvent(Loading, new GenericEventArgs<IEnumerable<PieceDrop>>(drops.Select(drop =>
                    new PieceDrop(drop.Position, sideConverter(drop.N)))));
                _loadingDrops = null;
            }
            else if (command.StartsWith("END", StringComparison.Ordinal))
            {
                RaiseEvent(Ended);
            }
            else if (command.StartsWith("ABOUT", StringComparison.Ordinal))
            {
                RaiseEvent(AboutRequested);
            }
            else if (_loadingDrops != null)
            {
                _loadingDrops.Add(command);
            }
        }

        private void HandleStartCommand(string command)
        {
            var size = Convert.ToInt32(command.Substring(5));
            RaiseEvent(Initailizing, new GenericEventArgs<int>(size));
        }

        private void HandleTurnCommand(string command)
        {
            RaiseEvent(Dropped, new GenericEventArgs<BoardPosition>(command.Substring(4).AsBoardPosition()));
        }
    }
}
