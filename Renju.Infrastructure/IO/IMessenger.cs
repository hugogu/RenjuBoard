namespace Renju.Infrastructure.IO
{
    using System;
    using System.Threading.Tasks;
    using Events;

    public interface IMessenger<REQ, RES> : IDisposable
    {
        event EventHandler<GenericEventArgs<RES>> MessageReceived;

        Task SendAsync(REQ message);
    }
}
