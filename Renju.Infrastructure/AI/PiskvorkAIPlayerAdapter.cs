namespace Renju.Infrastructure.AI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using Events;
    using IO;
    using Model;
    using Protocols;
    using Protocols.Piskvork;

    public class PiskvorkAIPlayerAdapter : DisposableModelBase, IAIPlayer
    {
        private static readonly Regex DropPattern = new Regex(@"[\d]+,[\d]+");
        private readonly string _aifile;
        private readonly Process _process;
        private readonly Lazy<IMessenger<string, string>> _aiMessenger;

        public PiskvorkAIPlayerAdapter(string aiFile)
        {
            _aifile = aiFile;
            _process = StartProcess(aiFile);
            _aiMessenger = new Lazy<IMessenger<string, string>>(() =>
            {
                var messenger = CreateMessenger(_process);
                AutoDispose(messenger.GetResponsesStream().Subscribe(OnReceivingAIMessage));
                AutoDispose(messenger);

                return messenger;
            });

            AutoDispose(_process);
        }

        public event EventHandler<GenericEventArgs<BoardPosition>> Dropping;

        public event EventHandler<GenericEventArgs<AIInfo>> Introducing;

        public event EventHandler<GenericEventArgs<String>> Says;

        public void RequestAbout()
        {
            Messenger.SendAsync("ABOUT");
        }

        public void Begin()
        {
            Messenger.SendAsync("BEGIN");
        }

        public void End()
        {
            Messenger.SendAsync("END");
        }

        public void Info(GameInfo info)
        {
            foreach (var message in info.ToMessages())
                Messenger.SendAsync(message);
        }

        public void Initialize(int boardSize)
        {
            Messenger.SendAsync("START "+ boardSize);
        }

        public void Load(IEnumerable<PieceDrop> drops)
        {
            Messenger.SendAsync("BOARD");
            Messenger.SendAsync("DONE");
        }

        public void OpponentDrops(PieceDrop stone)
        {
            Messenger.SendAsync("TURN " + stone.AsString());
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                Messenger.SendAsync("\0x1a");
            }
            base.Dispose(disposing);
        }

        protected IMessenger<string, string> Messenger
        {
            get { return _aiMessenger.Value; }
        }

        protected virtual Process StartProcess(string executionFile)
        {
            var processInfo = new ProcessStartInfo();
            processInfo.FileName = executionFile;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.CreateNoWindow = true;
            processInfo.ErrorDialog = false;

            return Process.Start(processInfo);
        }

        protected virtual IMessenger<string, string> CreateMessenger(Process process)
        {
            return Messengers.FromProcess<string, string>(process);
        }

        private void OnReceivingAIMessage(string airesponse)
        {
            var drop = DropPattern.Match(airesponse);
            if (drop.Success)
            {
                RaiseEvent(Dropping, new GenericEventArgs<BoardPosition>(drop.Value.AsBoardPosition()));
            }
            else
            {
                RaiseEvent(Says, new GenericEventArgs<string>(airesponse));
            }
        }
    }
}
