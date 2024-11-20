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

        public IEnumerable<Movie> GetFilteredMovies(string filterType, string filterValue)
        {
            IEnumerable<Movie> movies = AllMovies;

            switch (filterType.ToLower())
            {
                case "genre":
                    movies = movies.Where(m => m.Genres.Any(g => g.Name == filterValue));
                    break;

                case "director":
                    movies = movies.Where(m => m.Director != null && m.Director.Name == filterValue);
                    break;

                case "country":
                    movies = movies.Where(m => m.Countries.Any(c => c.Name == filterValue));
                    break;

                case "year":
                    if (int.TryParse(filterValue, out int year))
                    {
                        movies = movies.Where(m => m.ReleaseDate.Year == year);
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid filter type");
            }

            return movies;
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

        public void Delete(int id)
        {
            var movie = GetById(id);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
        }
    }
}
