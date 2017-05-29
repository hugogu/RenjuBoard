namespace Renju.Infrastructure.Events
{
    using System;
    using System.Diagnostics;

    public class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs(T message)
        {
            Debug.Assert(message != null);

            Message = message;
        }

        public T Message { get; protected internal set; }
    }
}
