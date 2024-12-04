using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Interfaces
{
    public interface IAllMovies
    {
        IEnumerable<Movie> AllMovies { get; }
        int GetTotalCount();
        PagedList<Movie> GetPagedMovies(string genre, string year, string director, string country, string sortBy, int pageNumber, int pageSize);
        Movie GetById(int id);
        void Create(Movie movie);
        void Update(Movie movie);
        void Delete(Movie movie);
    }
}
