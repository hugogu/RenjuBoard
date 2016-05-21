namespace Renju.Infrastructure.Events
{
    using Model;
    using Prism.Events;

    public class EvaluatedPointEvent : PubSubEvent<IReadOnlyBoardPoint>
    {
    }
}
