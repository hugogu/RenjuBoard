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
        private static readonly Regex InfoItemPattern = new Regex("\\w+=\"[^ \"]+\"");
        private readonly Lazy<Process> _process;
        private readonly Lazy<IMessenger<string, string>> _aiMessenger;
        private readonly AIInfo _info = new AIInfo();

        public PiskvorkAIPlayerAdapter(string aiFile)
        {
            _process = new Lazy<Process>(() => StartProcess(aiFile));
            _aiMessenger = new Lazy<IMessenger<string, string>>(CreateMessenger);
        }

        public event EventHandler<GenericEventArgs<BoardPosition>> Dropping;

        public event EventHandler<GenericEventArgs<String>> Says;

        public AIInfo AIInfo
        {
            get { return _info; }
        }

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

        protected Process Process
        {
            get { return _process.Value; }
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

            var process = Process.Start(processInfo);
            process.Exited += OnAIProcessExit;
            AutoDispose(process);

            RequestAbout();

            return process;
        }

        protected virtual IMessenger<string, string> CreateMessenger(Process process)
        {
            return Messengers.FromProcess<string, string>(process);
        }

        private void OnAIProcessExit(object sender, EventArgs e)
        {
            RaiseEvent(Says, new GenericEventArgs<string>("I'm dead!"));
        }

        private IMessenger<string, string> CreateMessenger()
        {
            var messenger = CreateMessenger(Process);
            AutoDispose(messenger.GetResponsesStream().Subscribe(OnReceivingAIMessage));
            AutoDispose(messenger);

            return messenger;
        }

        private void OnReceivingAIMessage(string airesponse)
        {
            var drop = DropPattern.Match(airesponse);
            if (drop.Success)
            {
                RaiseEvent(Dropping, new GenericEventArgs<BoardPosition>(drop.Value.AsBoardPosition()));
            }
            else if (airesponse.StartsWith("MESSAGE") || airesponse.StartsWith("DEBUG") || airesponse.StartsWith("ERROR"))
            {
                RaiseEvent(Says, new GenericEventArgs<string>(airesponse));
            }
            else if (airesponse.Contains("="))
            {
                var items = InfoItemPattern.Matches(airesponse);
                foreach(Match match in items)
                {
                    var item = match.Value;
                    var key = item.Substring(0, item.IndexOf('='));
                    var value = item.Substring(item.IndexOf('"') + 1).TrimEnd('"');
                    if (key == "name")
                        _info.Name = value;
                    else if (key == "author")
                        _info.Author = value;
                    else if (key == "country")
                        _info.Country = value;
                    else if (key == "version")
                        _info.Version = value;
                }
            }
        }
    }
}
