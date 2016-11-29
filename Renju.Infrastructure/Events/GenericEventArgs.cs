namespace Renju.Infrastructure.Events
{
    using System;
    using System.Diagnostics.Contracts;

    public class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs(T message)
        {
            Contract.Requires(message != null);

            Message = message;
        }

        public T Message { get; protected internal set; }
    }
}
