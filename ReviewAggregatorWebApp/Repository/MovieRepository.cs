using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

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

        public int GetTotalCount() => _context.Movies.Count();

        public PagedList<Movie> GetPagedMovies(string genre, string year, string director, string country, string sortBy, int pageNumber, int pageSize)
        {
            IQueryable<Movie> movies = _context.Movies
                .Include(x => x.Director)
                .Include(x => x.Genres)
                .Include(x => x.Countries);

            if (!string.IsNullOrWhiteSpace(genre))
            {
                movies = movies.Where(m => m.Genres.Any(g => g.Name.ToLower() == genre.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(director))
            {
                movies = movies.Where(m => m.Director != null && m.Director.Name.ToLower() == director.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                movies = movies.Where(m => m.Countries.Any(c => c.Name.ToLower() == country.ToLower()));
            }

            if (int.TryParse(year, out int parsedYear))
            {
                movies = movies.Where(m => m.ReleaseDate.Year == parsedYear);
            }

            var totalCount = movies.Count();
            switch (sortBy?.ToLower())
            {
                case "rating":
                    movies = movies.OrderByDescending(m => m.Rating);
                    break;
                case "date":
                    movies = movies.OrderByDescending(m => m.ReleaseDate);
                    break;
            }
            var pagedMovies = movies.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<Movie>(pagedMovies, totalCount, pageNumber, pageSize);
        }

        public IEnumerable<Movie> GetMoviesByTitlePrefix(string titlePrefix)
        {
            if (string.IsNullOrWhiteSpace(titlePrefix))
            {
                return null;
            }

            return _context.Movies
                .Include(x => x.Director)
                .Include(x => x.Genres)
                .Include(x => x.Countries)
                .Where(m => m.Name.StartsWith(titlePrefix.ToLower()))
                .Take(20)
                .ToList();
        }

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

        public void Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
            _context.SaveChanges();
        }
    }
}
