using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Interfaces
{
    public interface IAllReviews
    {
        IEnumerable<Review> AllReviews { get; }
        IEnumerable<Review> GetByMovieId(int movieId);
        Review GetById(int id);
        void Create(Review review);
        void Update(Review review);
        void Delete(int id);
    }
}
