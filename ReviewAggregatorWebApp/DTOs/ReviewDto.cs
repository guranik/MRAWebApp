namespace ReviewAggregatorWebApp.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int MovieId { get; set; }
        public DateTime PostDate { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = null!;
    }
}
