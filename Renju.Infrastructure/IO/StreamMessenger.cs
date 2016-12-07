namespace Renju.Infrastructure.IO
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Events;

    public class StreamMessenger<REQ, RES> : DisposableModelBase, IMessenger<REQ, RES>
    {
        private static readonly TypeConverter _responseConverter = TypeDescriptor.GetConverter(typeof(RES));
        private static readonly TypeConverter _requestConverter = TypeDescriptor.GetConverter(typeof(REQ));
        private readonly ActionBlock<String> _writeBlock;

        static StreamMessenger()
        {
            Contract.Assume(_requestConverter.CanConvertTo(typeof(string)), "Request message type must be convertable to String.");
            Contract.Assume(_responseConverter.CanConvertFrom(typeof(string)), "Response message type must be convertable from String.");
        }

        public StreamMessenger(StreamReader inputReader, StreamWriter outputWriter)
        {
            Contract.Requires(inputReader != null);
            Contract.Requires(outputWriter != null);

            _writeBlock = new ActionBlock<string>(async s => await outputWriter.WriteLineAsync(s).ConfigureAwait(false));
            _writeBlock.Completion.ContinueWith(_ => outputWriter.Dispose());

            AutoDispose(inputReader.ReadAllLines().Subscribe(OnReceivingStdOut));
            AutoDispose(inputReader);
            AutoDispose(outputWriter);
        }

        public StreamMessenger(Stream inputStream, Stream outputStream)
            : this(new StreamReader(inputStream), new StreamWriter(outputStream))
        {
            Contract.Requires(inputStream != null);
            Contract.Requires(outputStream != null);
            Contract.Requires(inputStream.CanRead, "input stream should be readable.");
            Contract.Requires(outputStream.CanWrite, "output stream should be writable.");
        }

        public virtual event EventHandler<GenericEventArgs<RES>> MessageReceived;

        public async virtual Task SendAsync(REQ message)
        {
            var messageText = _requestConverter.ConvertToString(message);
            if (!await _writeBlock.SendAsync(messageText).ConfigureAwait(false))
            {
                throw new ObjectDisposedException(nameof(_writeBlock));
            }
        }

        protected virtual void OnReceivingStdOut(string message)
        {
            var respose = (RES)_responseConverter.ConvertFromString(message);
            RaiseEvent(MessageReceived, new GenericEventArgs<RES>(respose));
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                _writeBlock.Complete();
                _writeBlock.Completion.Wait();
            }

            base.Dispose(disposing);
        }
    }
}
