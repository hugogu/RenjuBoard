namespace Renju.Infrastructure.Protocols.Piskvork
{
    using System;
    using System.Collections.Generic;
    using AI;
    using Model;

    public static class PiskvorkMessageUtils
    {
        public static BoardPosition AsBoardPosition(this string message)
        {
            var axis = message.Split(',');
            var x = Convert.ToInt32(axis[0]);
            var y = Convert.ToInt32(axis[1]);

            return new BoardPosition(x, y);
        }

        public static string AsString(this BoardPosition position)
        {
            return position.X + "," + position.Y;
        }

        public static string AsString(this PieceDrop drop)
        {
            return drop.X + "," + drop.Y;
        }

        public static IEnumerable<string> ToMessages(this GameInfo info)
        {
            yield return "INFO timeout_match " + info.MatchTime;
            yield return "INFO timeout_turn "  + info.TurnTime;
            yield return "INFO max_memory "    + info.MaximumMemory;
            yield return "INFO time_left "     + info.TimeLeft;
            yield return "INFO folder "        + info.TempFolder;
            yield return "INFO evaluate "      + info.CurrentMousePosition.AsString();
            yield return "INFO game_type "     + info.OpponentType;
        }
    }
}
