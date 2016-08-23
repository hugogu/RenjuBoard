namespace Renju.Infrastructure.IO
{
    using System;
    using System.Reactive.Linq;
    using Events;

    public static class MessengerExtensions
    {
        public static IObservable<RES> GetResponsesStream<REQ, RES>(this IMessenger<REQ, RES> messenger)
        {
            return Observable.FromEventPattern<GenericEventArgs<RES>>(
                    handler => messenger.MessageReceived += handler,
                    handler => messenger.MessageReceived -= handler)
                   .Select(p => p.EventArgs.Message);
        }
    }
}
