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
    using Model;
    using Protocols;

    public class PiskvorkAIPlayerAdapter : DisposableModelBase, IAIController
    {
        private static readonly Regex DropPattern = new Regex(@"[\d]+,[\d]+");
        private static readonly Regex InfoItemPattern = new Regex("\\w+=\"[^ \"]+\"");
        private readonly Lazy<Process> _process;
        private readonly Lazy<IMessenger<string, string>> _aiMessenger;

        public PiskvorkAIPlayerAdapter(string aiFile)
        {
            Debug.Assert(aiFile != null);

            _process = new Lazy<Process>(() => StartProcess(aiFile));
            _aiMessenger = new Lazy<IMessenger<string, string>>(CreateMessenger);
        }

        public event EventHandler<GenericEventArgs<BoardPosition>> Dropping;

        public event EventHandler<GenericEventArgs<string>> Says;

        public AIInfo AIInfo { get; } = new AIInfo();

        public async Task RequestAboutAsync()
        {
            await Messenger.SendAsync("ABOUT").ConfigureAwait(false);
        }

        public async Task BeginAsync()
        {
            Debug.Assert(_process.IsValueCreated);
            await Messenger.SendAsync("BEGIN").ConfigureAwait(false);
        }

        public async Task EndAsync()
        {
            Debug.Assert(_process.IsValueCreated);
            await Messenger.SendAsync("END").ConfigureAwait(false);
        }

        public async Task InfoAsync(GameInfo info)
        {
            foreach (var message in info.ToMessages())
                await Messenger.SendAsync(message).ConfigureAwait(false);
        }

        public async Task InitializeAsync(int boardSize)
        {
            await Messenger.SendAsync("START " + boardSize).ConfigureAwait(false);
        }

        public async Task LoadAsync(IEnumerable<PieceDrop> drops)
        {
            Debug.Assert(_process.IsValueCreated);
            await Messenger.SendAsync("BOARD").ConfigureAwait(false);
            await Messenger.SendAsync("DONE").ConfigureAwait(false);
        }

        public async Task OpponentDropsAsync(BoardPosition stone)
        {
            Debug.Assert(_process.IsValueCreated);
            await Messenger.SendAsync("TURN " + stone.AsString()).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    // Send Stream Ending to Terminate the process.
                    Messenger.SendAsync("\0x1a").ConfigureAwait(false);
                }
                catch (InvalidOperationException)
                {
                    Messenger.Dispose();
                    Process.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        protected IMessenger<string, string> Messenger
        {
            get { return _aiMessenger.Value; }
        }

        protected internal Process Process
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

            Task.Run(async () => await RequestAboutAsync().ConfigureAwait(false));

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
                        AIInfo.Name = value;
                    else if (key == "author")
                        AIInfo.Author = value;
                    else if (key == "country")
                        AIInfo.Country = value;
                    else if (key == "version")
                        AIInfo.Version = value;
                }
            }
        }
    }
}
