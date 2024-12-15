using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.BusinessLogic
{
    public class RatingService : IRatingService
    {
        private IAllMovies _movieRepository;
        private IAllReviews _reviewRepository;
        public RatingService(IAllReviews reviewRepository, IAllMovies movierepository) 
        {
            _movieRepository = movierepository;
            _reviewRepository = reviewRepository;
        }

        public void CalculateRating(int movieId)
        {
            List<int> ratings = _reviewRepository
                .GetByMovieId(movieId)
                .Select(r => r.Rating).ToList();

            Movie movie = _movieRepository.GetById(movieId);

            if (ratings.Count() != 0)
            {
                movie.Rating = (decimal)ratings.Average();
                _movieRepository.Update(movie);
            }
            else
            {
                throw new Exception("У фильма нет отзывов.");
            }
        }
    }
}
