using Microsoft.Practices.Unity;
using Renju.Infrastructure.Model;

namespace Renju.Infrastructure.Events
{
    public class NewGameOptions
    {
        public int BoardSize { get; set; } = 15;

        [Dependency]
        public IGameRuleEngine RuleEngine { get; internal set; }

        public static NewGameOptions Default { get; } = new NewGameOptions();
    }
}
