namespace Renju.Infrastructure.IO
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.Practices.Unity.Utility;

    public class StreamMessenger<REQ, RES> : DisposableModelBase, IMessenger<REQ, RES>
    {
        private static readonly TypeConverter _responseConverter = TypeDescriptor.GetConverter(typeof(RES));
        private static readonly TypeConverter _requestConverter = TypeDescriptor.GetConverter(typeof(REQ));
        private readonly StreamReader _inputReader;
        private readonly StreamWriter _outputWriter;

        public StreamMessenger(Stream inputStream, Stream outputStream)
        {
            Guard.ArgumentNotNull(inputStream, "inputStream");
            Guard.ArgumentNotNull(outputStream, "outputStream");
            Debug.Assert(inputStream.CanRead, "input stream should be readable.");
            Debug.Assert(outputStream.CanWrite, "output stream should be writable.");

            if (!_responseConverter.CanConvertFrom(typeof(string)))
                throw new ArgumentException(String.Format("Response message type {0} must be convertable from String.", typeof(RES)));

            if (!_requestConverter.CanConvertTo(typeof(string)))
                throw new ArgumentException(String.Format("Request message type {0} must be convertable to String.", typeof(REQ)));

            _inputReader = new StreamReader(inputStream);
            _outputWriter = new StreamWriter(outputStream);

            AutoDispose(_inputReader);
            AutoDispose(_outputWriter);

            ListenOnInputStream();
        }

        public virtual event EventHandler<GenericEventArgs<RES>> MessageReceived;

        public async virtual Task SendAsync(REQ message)
        {
            Guard.ArgumentNotNull(message, "message");
            var messageText = _requestConverter.ConvertToString(message);
            await _outputWriter.WriteLineAsync(messageText);
        }

        protected async void ListenOnInputStream()
        {
            while (!_inputReader.EndOfStream)
            {
                var line = await _inputReader.ReadLineAsync();
                RaiseMessageReceivedEvent((RES)_responseConverter.ConvertFromString(line));
            }
        }

        protected virtual void RaiseMessageReceivedEvent(RES message)
        {
            RaiseEvent(MessageReceived, new GenericEventArgs<RES>(message));
        }
    }
}
