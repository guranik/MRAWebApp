using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    public class MoviesListController : Controller
    {
        private readonly IMemoryCache _cache;

        public MoviesListController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Filter(string filterType, string filterValue)
        {

            if (!_cache.TryGetValue("movies", out List<Movie> movies))
            {
                return NotFound("Movies not found in cache.");
            }

            IEnumerable<Movie> filteredMovies = movies;

            switch (filterType.ToLower())
            {
                case "genre":
                    filteredMovies = movies.Where(m => m.Genres.Any(g => g.Name == filterValue));
                    ViewBag.FilterTitle = $"Фильмы жанра {filterValue}";
                    break;

                case "director":
                    filteredMovies = movies.Where(m => m.Director != null && m.Director.Name == filterValue);
                    ViewBag.FilterTitle = $"Фильмы режиссера {filterValue}";
                    break;

                case "country":
                    filteredMovies = movies.Where(m => m.Countries.Any(c => c.Name == filterValue));
                    ViewBag.FilterTitle = $"Фильмы страны {filterValue}";
                    break;

                case "year":
                    filteredMovies = movies.Where(m => m.ReleaseDate.Year.ToString() == filterValue);
                    ViewBag.FilterTitle = $"Фильмы {filterValue} года";
                    break;

                default:
                    return NotFound();
            }

            return View(filteredMovies);
        }
    }
}
