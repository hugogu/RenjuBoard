namespace Renju.Core
{
    using System.ComponentModel;
    using Infrastructure.Model;
    using Infrastructure.Protocols;

    [DisplayName("User Player")]
    public class HumanPlayer : IGamePlayer
    {
        [DisplayName("Author Name")]
        public string AuthorName { get; set; }

        [DisplayName("Country")]
        public string Country { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        public Side Side { get; set; }

        public void PlayOn(IBoardMonitor board)
        {
            /* Noop */
        }
    }
}
