namespace Renju.Infrastructure.Events
{
    using Prism.Events;
    using Model;

    public class ResolvingBoardEvent : PubSubEvent<IReadBoardState<IReadOnlyBoardPoint>>
    {
    }
}
