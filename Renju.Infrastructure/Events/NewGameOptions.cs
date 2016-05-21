namespace Renju.Infrastructure.Events
{
    using Microsoft.Practices.Unity;
    using Model;

    public class NewGameOptions
    {
        public static NewGameOptions Default { get; } = new NewGameOptions();

        public int BoardSize { get; set; } = 15;

        [Dependency]
        public IGameRuleEngine RuleEngine { get; internal set; }
    }
}
