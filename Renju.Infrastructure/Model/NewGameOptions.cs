namespace Renju.Infrastructure.Model
{
    using Microsoft.Practices.Unity;

    public class NewGameOptions
    {
        public static NewGameOptions Default { get; } = new NewGameOptions();

        public int BoardSize { get; set; } = 15;

        [Dependency]
        public IGameRuleEngine RuleEngine { get; internal set; }
    }
}
