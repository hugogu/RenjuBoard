using Prism.Events;
using Renju.Infrastructure.Model;

namespace Renju.Infrastructure.Events
{
    public class ResolvingBoardEvent : PubSubEvent<IReadBoardState<IReadOnlyBoardPoint>>
    {
    }
}
