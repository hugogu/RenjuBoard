namespace Renju.Infrastructure.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Events;

    [ContractClassFor(typeof(IMessenger<, >))]
    public abstract class MessengerContract<REQ, RES> : IMessenger<REQ, RES>
    {
        event EventHandler<GenericEventArgs<RES>> IMessenger<REQ, RES>.MessageReceived
        {
            add { }
            remove { }
        }

        void IDisposable.Dispose()
        {
        }

        Task IMessenger<REQ, RES>.SendAsync(REQ message)
        {
            Contract.Requires(message != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }
    }
}
