using Microsoft.AspNetCore.Mvc;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IAllMovies _movieRepository;

        public MoviesController(IAllMovies movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public IActionResult Filter(string filterType, string filterValue)
        {
            IEnumerable<Movie> movies = _movieRepository.AllMovies;
            string filterTitle = string.Empty;

            switch (filterType.ToLower())
            {
                case "genre":
                    movies = movies.Where(m => m.Genres.Any(g => g.Name == filterValue));
                    filterTitle = $"Фильмы жанра {filterValue}";
                    break;

                case "director":
                    movies = movies.Where(m => m.Director.Name == filterValue);
                    filterTitle = $"Фильмы режиссера {filterValue}";
                    break;

                case "country":
                    movies = movies.Where(m => m.Countries.Any(c => c.Name == filterValue));
                    filterTitle = $"Фильмы страны {filterValue}";
                    break;

                case "year":
                    if (int.TryParse(filterValue, out int year))
                    {
                        movies = movies.Where(m => m.ReleaseDate.Year == year);
                        filterTitle = $"Фильмы {year} года";
                    }
                    break;

                default:
                    return NotFound();
            }

            ViewBag.FilterTitle = filterTitle;
            return View(movies);
        }
    }
}