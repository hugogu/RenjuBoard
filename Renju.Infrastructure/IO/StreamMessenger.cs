namespace Renju.Infrastructure.IO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.Practices.Unity.Utility;

    public class StreamMessenger<REQ, RES> : DisposableModelBase, IMessenger<REQ, RES>
    {
        private static readonly TypeConverter _responseConverter = TypeDescriptor.GetConverter(typeof(RES));
        private static readonly TypeConverter _requestConverter = TypeDescriptor.GetConverter(typeof(REQ));
        private readonly StreamReader _inputReader;
        private readonly StreamWriter _outputWriter;

        public StreamMessenger(StreamReader inputReader, StreamWriter outputWriter)
        {
            Guard.ArgumentNotNull(inputReader, "inputReader");
            Guard.ArgumentNotNull(outputWriter, "outputWriter");

            _inputReader = inputReader;
            _outputWriter = outputWriter;

            AutoDispose(ReadLinesFromInputReader().ToObservable(TaskPoolScheduler.Default).Subscribe(OnReceivingStdOut));
            AutoDispose(_inputReader);
            AutoDispose(_outputWriter);
        }

        public StreamMessenger(Stream inputStream, Stream outputStream)
            : this(new StreamReader(inputStream), new StreamWriter(outputStream))
        {
            Guard.ArgumentNotNull(inputStream, "inputStream");
            Guard.ArgumentNotNull(outputStream, "outputStream");
            Debug.Assert(inputStream.CanRead, "input stream should be readable.");
            Debug.Assert(outputStream.CanWrite, "output stream should be writable.");

            if (!_responseConverter.CanConvertFrom(typeof(string)))
                throw new ArgumentException(String.Format("Response message type {0} must be convertable from String.", typeof(RES)));

            if (!_requestConverter.CanConvertTo(typeof(string)))
                throw new ArgumentException(String.Format("Request message type {0} must be convertable to String.", typeof(REQ)));
        }

        public virtual event EventHandler<GenericEventArgs<RES>> MessageReceived;

        public async virtual Task SendAsync(REQ message)
        {
            Guard.ArgumentNotNull(message, "message");
            var messageText = _requestConverter.ConvertToString(message);
            await _outputWriter.WriteLineAsync(messageText);
        }

        protected virtual void OnReceivingStdOut(string message)
        {
            RaiseMessageReceivedEvent((RES)_responseConverter.ConvertFromString(message));
        }

        protected virtual void RaiseMessageReceivedEvent(RES message)
        {
            RaiseEvent(MessageReceived, new GenericEventArgs<RES>(message));
        }

        private IEnumerable<string> ReadLinesFromInputReader()
        {
            while (!_inputReader.EndOfStream)
            {
                yield return _inputReader.ReadLine();
            }
        }
    }
}
