using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Repository
{
    public class ReviewRepository : IAllReviews
    {
        private readonly Db8428Context _context;
        public ReviewRepository(Db8428Context context)
        {
            _context = context;
        }

        public IEnumerable<Review> AllReviews => _context.Reviews
            .Include(x => x.User)
            .Include(x => x.Movie);

        public Review GetById(int id) => _context.Reviews
            .Include(x => x.User)
            .Include(x => x.Movie)
            .FirstOrDefault(x => x.Id == id)
            ?? throw new InvalidOperationException($"Review with ID {id} not found.");
        public IEnumerable<Review> GetByMovieId(int movieId) => _context.Reviews
            .Where(x => x.MovieId == movieId)
            .Include(x=> x.User)
            .Include(x=> x.Movie);
    }
}
