namespace Renju.Core
{
    public interface IReadOnlyBoardPoint
    {
        BoardPosition Position { get; }

        int? Index { get; }

        Side? Status { get; }

        // TODO: Remvoe setter of Weight
        int Weight { get; set; }
    }
}
