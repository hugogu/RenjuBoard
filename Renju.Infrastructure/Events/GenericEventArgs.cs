namespace Renju.Infrastructure.Events
{
    using System;
    using Microsoft.Practices.Unity.Utility;

    public class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs(T message)
        {
            Guard.ArgumentNotNull(message, "message");

            Message = message;
        }

        public T Message { get; protected internal set; }
    }
}
