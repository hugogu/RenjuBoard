namespace Renju.Infrastructure.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Events;

    [ContractClass(typeof(MessengerContract<, >))]
    public interface IMessenger<REQ, RES> : IDisposable
    {
        event EventHandler<GenericEventArgs<RES>> MessageReceived;

        Task SendAsync(REQ message);
    }
}
