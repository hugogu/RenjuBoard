using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renju.Core
{
    public interface IGameRule
    {
        string Name { get; }

        bool? CanDropOn(GameBoard board, PieceDrop drop);

        bool? Win(GameBoard board, PieceDrop drop);

        Side? NextSide(GameBoard board);
    }
}
