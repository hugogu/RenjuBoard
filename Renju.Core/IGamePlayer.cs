namespace Renju.Core
{
    public interface IGamePlayer
    {
        Side Side { get; set; }

        GameBoard Board { get; set; }
    }
}
