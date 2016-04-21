using System.Collections.Generic;
using Renju.Infrastructure.Model;

namespace Renju.Infrastructure.Events
{
    public class NewGameOptions
    {
        public bool AIFirst { get; set; }

        public int BoardSize { get; set; } = 15;

        public IEnumerable<IGameRule> Rules { get; set; }
    }
}
