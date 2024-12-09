
namespace ReviewAggregatorWebApp.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? PosterLink { get; set; }

        public int? DirectorId { get; set; }

        public DateTime ReleaseDate { get; set; }

        public decimal Rating { get; set; }
    }
}