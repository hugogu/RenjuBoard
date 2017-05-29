namespace Renju.Core
{
    using System.ComponentModel;
    using Infrastructure.Model;
    using Infrastructure.Protocols;
    using Microsoft.Practices.Unity;

    [DisplayName("User Player")]
    public class HumanPlayer : IGamePlayer
    {
        [OptionalDependency]
        [DisplayName("Author Name")]
        public string AuthorName { get; set; }

        [OptionalDependency]
        [DisplayName("Country")]
        public string Country { get; set; }

        [OptionalDependency]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Dependency]
        public Side Side { get; set; }

        public virtual void PlayOn(IBoardMonitor board)
        {
            /* Noop */
        }
    }
}
