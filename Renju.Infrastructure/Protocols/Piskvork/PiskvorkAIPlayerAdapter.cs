namespace Renju.Infrastructure.Protocols.Piskvork
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AI;
    using Events;
    using IO;
    using Microsoft.Practices.Unity.Utility;
    using Model;
    using Protocols;

    public class PiskvorkAIPlayerAdapter : DisposableModelBase, IAIController
    {
        private static readonly Regex DropPattern = new Regex(@"[\d]+,[\d]+");
        private static readonly Regex InfoItemPattern = new Regex("\\w+=\"[^ \"]+\"");
        private readonly Lazy<Process> _process;
        private readonly Lazy<IMessenger<string, string>> _aiMessenger;
        private readonly AIInfo _info = new AIInfo();

        public PiskvorkAIPlayerAdapter(string aiFile)
        {
            Guard.ArgumentNotNull(aiFile, "aiFile");

            _process = new Lazy<Process>(() => StartProcess(aiFile));
            _aiMessenger = new Lazy<IMessenger<string, string>>(CreateMessenger);
        }

        public event EventHandler<GenericEventArgs<BoardPosition>> Dropping;

        public event EventHandler<GenericEventArgs<string>> Says;

        public AIInfo AIInfo
        {
            get { return _info; }
        }

        public async Task RequestAbout()
        {
            await Messenger.SendAsync("ABOUT");
        }

        public async Task Begin()
        {
            Debug.Assert(_process.IsValueCreated);
            await Messenger.SendAsync("BEGIN");
        }

        public async Task End()
        {
            Debug.Assert(_process.IsValueCreated);
            await Messenger.SendAsync("END");
        }

        public async Task Info(GameInfo info)
        {
            Guard.ArgumentNotNull(info, "info");
            foreach (var message in info.ToMessages())
                await Messenger.SendAsync(message);
        }

        public async Task Initialize(int boardSize)
        {
            await Messenger.SendAsync("START " + boardSize);
        }

        public async Task Load(IEnumerable<PieceDrop> drops)
        {
            Debug.Assert(_process.IsValueCreated);
            Guard.ArgumentNotNull(drops, "drops");
            await Messenger.SendAsync("BOARD");
            await Messenger.SendAsync("DONE");
        }

        public async Task OpponentDrops(BoardPosition stone)
        {
            Debug.Assert(_process.IsValueCreated);
            Guard.ArgumentNotNull(stone, "stone");
            await Messenger.SendAsync("TURN " + stone.AsString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
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

            Task.Run(async () => await RequestAbout());

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
            
            if (airesponse.StartsWith("MESSAGE") || airesponse.StartsWith("DEBUG") || airesponse.StartsWith("ERROR"))
            {
                RaiseEvent(Says, new GenericEventArgs<string>(airesponse));
            }
            else if (drop.Success)
            {
                RaiseEvent(Dropping, new GenericEventArgs<BoardPosition>(drop.Value.AsBoardPosition()));
            }
            else if (airesponse.Contains("="))
            {
                var items = InfoItemPattern.Matches(airesponse);
                foreach (Match match in items)
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
