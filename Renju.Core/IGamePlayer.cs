namespace Renju.Core
{
    public interface IGamePlayer
    {
        Side Side { get; set; }

        IGameBoard Board { get; set; }
    }
}
