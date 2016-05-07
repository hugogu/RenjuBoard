namespace Renju.Infrastructure.Model.Extensions
{
    public static class PointExtensions
    {
        public static string GetLiternalPresentation(this Side? side)
        {
            return side == null ? "_" : (side == Side.Black ? "●" : "○");
        }
    }
}
