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

        public int GetCountByMovie(int movieId) => _context.Reviews.Where(x => x.MovieId == movieId).Count();

        public PagedList<Review> GetPagedReviews(int movieId, int pageNumber, int pageSize)
        {
            IQueryable<Review> reviews = _context.Reviews
                .Where(x => x.MovieId == movieId)
                .Include(x => x.User)
                .Include(x => x.Movie);

            var totalCount = reviews.Count();

            var pagedReviews = reviews.OrderByDescending(x => x.PostDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<Review>(pagedReviews,totalCount,pageNumber,pageSize);
        }

        public Review GetById(int id) => _context.Reviews
            .Include(x => x.User)
            .Include(x => x.Movie)
            .FirstOrDefault(x => x.Id == id)
            ?? throw new InvalidOperationException($"Review with ID {id} not found.");

        public IEnumerable<Review> GetByMovieId(int movieId) => _context.Reviews
            .Where(x => x.MovieId == movieId)
            .Include(x => x.User)
            .Include(x => x.Movie);

        public void Create(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void Delete(Review review)
        {
            _context.Reviews.Remove(review);
            _context.SaveChanges();
        }
    }
}
