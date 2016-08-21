namespace Renju.Infrastructure.IO
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.Practices.Unity.Utility;

    public class StreamMessanger<T> : DisposableModelBase, IMessanger<T>
    {
        private static readonly TypeConverter MessageConverter = TypeDescriptor.GetConverter(typeof(T));
        private readonly StreamReader _inputReader;
        private readonly StreamWriter _outputWriter;

        public StreamMessanger(Stream inputStream, Stream outputStream)
        {
            Guard.ArgumentNotNull(inputStream, "inputStream");
            Guard.ArgumentNotNull(outputStream, "outputStream");
            Debug.Assert(inputStream.CanRead, "input stream should be readable.");
            Debug.Assert(outputStream.CanWrite, "output stream should be writable.");

            if (!MessageConverter.CanConvertFrom(typeof(string)))
                throw new ArgumentException("Generic message type must convertable from String.");

            _inputReader = new StreamReader(inputStream);
            _outputWriter = new StreamWriter(outputStream);

            AutoDispose(_inputReader);
            AutoDispose(_outputWriter);

            ListenOnInputStream();
        }

        public virtual event EventHandler<GenericEventArgs<T>> MessageReceived;

        public async virtual Task SendAsync(T message)
        {
            Guard.ArgumentNotNull(message, "message");
            var messageText = MessageConverter.ConvertToString(message);
            await _outputWriter.WriteLineAsync(messageText);
        }

        protected async void ListenOnInputStream()
        {
            var line = await _inputReader.ReadLineAsync();
            RaiseMessageReceivedEvent((T)MessageConverter.ConvertFromString(line));
        }

        protected virtual void RaiseMessageReceivedEvent(T message)
        {
            RaiseEvent(MessageReceived, new GenericEventArgs<T>(message));
        }
    }
}
