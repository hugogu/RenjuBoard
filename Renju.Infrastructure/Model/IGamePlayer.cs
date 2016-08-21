namespace Renju.Infrastructure.Model
{
    public interface IGamePlayer
    {
        Side Side { get; set; }

        string Name { get; set; }

        string AuthorName { get;set; }

        string Country { get; set; }
    }
}
