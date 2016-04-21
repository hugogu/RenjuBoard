using Prism.Events;

namespace Renju.Infrastructure.Events
{
    public class StartNewGameEvent : PubSubEvent<NewGameOptions>
    {
    }
}
