namespace Renju.Core
{
    using Infrastructure.Model;
    using Infrastructure.Protocols;

    public class HumanPlayer : IGamePlayer
    {
        public HumanPlayer(string authorName, string country, string name, Side side)
        {
            AuthorName = authorName;
            Country = country;
            Name = name;
            Side = side;
        }

        public string AuthorName { get; set; }

        public string Country { get; set; }

        public string Name { get; set; }

        public Side Side { get; set; }

        public void PlayOn(IBoardMonitor board)
        {
            /* Noop */
        }
    }
}
