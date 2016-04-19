namespace Renju.Infrastructure.Model
{
    public interface IReadOnlyBoardPoint
    {
        BoardPosition Position { get; }

        int? Index { get; }

        Side? Status { get; }

        // TODO: Move Weight and RequiresReevaluateWeight to a more appropriate place as they are more for AI which is not about point
        int Weight { get; set; }

        bool RequiresReevaluateWeight { get; set; }
    }
}
