namespace Renju.Infrastructure.IO
{
    using System;
    using System.Threading.Tasks;
    using Events;

    public interface IMessanger<T>
    {
        event EventHandler<GenericEventArgs<T>> MessageReceived;

        Task SendAsync(T message);
    }
}
