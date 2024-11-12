namespace ReviewAggregatorWebApp.Middleware.ApiResponseData
{
    public class MovieDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public string EnName { get; set; }
        public int Year { get; set; }
        public RatingInfo Rating { get; set; }
        public List<GenreInfo> Genres { get; set; }
        public List<CountryInfo> Countries { get; set; }
        public PosterInfo Poster { get; set; }
        public List<PersonInfo> Persons { get; set; }
    }
}
