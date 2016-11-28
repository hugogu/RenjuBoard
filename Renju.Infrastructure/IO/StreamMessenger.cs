namespace Renju.Infrastructure.IO
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Events;
    using Microsoft.Practices.Unity.Utility;

    public class StreamMessenger<REQ, RES> : DisposableModelBase, IMessenger<REQ, RES>
    {
        private static readonly TypeConverter _responseConverter = TypeDescriptor.GetConverter(typeof(RES));
        private static readonly TypeConverter _requestConverter = TypeDescriptor.GetConverter(typeof(REQ));
        private readonly ActionBlock<String> _writeBlock;

        public StreamMessenger(StreamReader inputReader, StreamWriter outputWriter)
        {
            Guard.ArgumentNotNull(inputReader, nameof(inputReader));
            Guard.ArgumentNotNull(outputWriter, nameof(outputWriter));

            _writeBlock = new ActionBlock<string>(async s => await outputWriter.WriteLineAsync(s));
            _writeBlock.Completion.ContinueWith(_ => outputWriter.Dispose());

            AutoDispose(inputReader.ReadAllLines().Subscribe(OnReceivingStdOut));
            AutoDispose(inputReader);
            AutoDispose(outputWriter);
        }

        public StreamMessenger(Stream inputStream, Stream outputStream)
            : this(new StreamReader(inputStream), new StreamWriter(outputStream))
        {
            Guard.ArgumentNotNull(inputStream, nameof(inputStream));
            Guard.ArgumentNotNull(outputStream, nameof(outputStream));
            Debug.Assert(inputStream.CanRead, "input stream should be readable.");
            Debug.Assert(outputStream.CanWrite, "output stream should be writable.");

            if (!_responseConverter.CanConvertFrom(typeof(string)))
                throw new NotSupportedException(String.Format("Response message type {0} must be convertable from String.", typeof(RES)));

            if (!_requestConverter.CanConvertTo(typeof(string)))
                throw new NotSupportedException(String.Format("Request message type {0} must be convertable to String.", typeof(REQ)));
        }

        public virtual event EventHandler<GenericEventArgs<RES>> MessageReceived;

        public async virtual Task SendAsync(REQ message)
        {
            Guard.ArgumentNotNull(message, nameof(message));
            var messageText = _requestConverter.ConvertToString(message);
            if (!await _writeBlock.SendAsync(messageText))
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
