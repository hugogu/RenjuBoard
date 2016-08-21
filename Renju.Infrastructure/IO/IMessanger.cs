namespace Renju.Infrastructure.IO
{
    using System;
    using System.Threading.Tasks;
    using Events;

    public interface IMessanger<REQ, RES>
    {
        event EventHandler<GenericEventArgs<RES>> MessageReceived;

        Task SendAsync(REQ message);
    }
}
