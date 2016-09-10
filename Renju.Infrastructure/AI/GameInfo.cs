namespace Renju.Infrastructure.AI
{
    using Model;

    public class GameInfo
    {
        /// <summary>
        /// In ms.
        /// </summary>
        /// <returns></returns>
        public int TurnTime { get; set; }

        /// <summary>
        /// In ms.
        /// </summary>
        /// <returns></returns>
        public int MatchTime { get; set; }

        /// <summary>
        /// In bytes, 0 if no limitation.
        /// </summary>
        /// <returns></returns>
        public int MaximumMemory { get; set; }

        /// <summary>
        /// for whole match in ms.
        /// </summary>
        /// <returns></returns>
        public int TimeLeft { get; set; }

        public GameType OpponentType { get; set; }

        public string TempFolder { get; set; }

        public BoardPosition CurrentMousePosition { get; set; }
    }
}
