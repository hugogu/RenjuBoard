namespace Renju.Infrastructure.Events
{
    using Prism.Events;

    public class StartNewGameEvent : PubSubEvent<NewGameOptions>
    {
    }
}
