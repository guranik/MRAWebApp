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
    public class MovieRepository : IAllMovies
    {
        private readonly Db8428Context _context;
        public MovieRepository(Db8428Context context)
        {
            _context = context;
        }

        public IEnumerable<Movie> AllMovies => _context.Movies
            .Include(x => x.Director)
            .Include(x => x.Genres)
            .Include(x => x.Countries);

        public Movie GetById(int id) => _context.Movies
            .Include(x => x.Director)
            .Include(x => x.Genres)
            .Include(x => x.Countries)
            .FirstOrDefault(x => x.Id == id) ??
                throw new InvalidOperationException($"Movie with ID {id} not found.");

        public void Create(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();
        }

        public void Update(Movie movie)
        {
            _context.Movies.Update(movie);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var movie = GetById(id);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
        }
    }
}
