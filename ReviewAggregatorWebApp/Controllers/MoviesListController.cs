using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
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

        public IActionResult Filter(string genre = "", string year = "", string director = "", string country = "", string sortBy = "rating")
        {
            if (!_cache.TryGetValue("movies", out List<Movie>
            movies))
            {
                return NotFound("Movies not found in cache.");
            }
            if (!_cache.TryGetValue("genres", out List<Genre>
            genres))
            {
                return NotFound("Genres not found in cache.");
            }
            if (!_cache.TryGetValue("countries", out List<Country>
            countries))
            {
                return NotFound("Countries not found in cache.");
            }
            if (!_cache.TryGetValue("directors", out List<Director>
            directors))
            {
                return NotFound("Directors not found in cache.");
            }
            if (!_cache.TryGetValue("years", out List<int>
            years))
            {
                return NotFound("Years not found in cache.");
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                HttpContext.Session.SetString("SortOrder", sortBy);
            }
            else
            {
                sortBy = HttpContext.Session.GetString("SortOrder") ?? "rating";
            }

            ViewBag.SortOrder = sortBy;

            IEnumerable<Movie>
                filteredMovies = movies;

            if (!string.IsNullOrEmpty(genre))
            {
                filteredMovies = filteredMovies.Where(m => m.Genres.Any(g => g.Name == genre));
            }

            if (!string.IsNullOrEmpty(director))
            {
                filteredMovies = filteredMovies.Where(m => m.Director != null && m.Director.Name == director);
            }

            if (!string.IsNullOrEmpty(country))
            {
                filteredMovies = filteredMovies.Where(m => m.Countries.Any(c => c.Name == country));
            }

            if (!string.IsNullOrEmpty(year))
            {
                filteredMovies = filteredMovies.Where(m => m.ReleaseDate.Year.ToString() == year);
            }

            switch (sortBy?.ToLower())
            {
                case "rating":
                    filteredMovies = filteredMovies.OrderByDescending(m => m.Rating);
                    break;
                case "date":
                    filteredMovies = filteredMovies.OrderByDescending(m => m.ReleaseDate);
                    break;
            }

            // Передаем данные для фильтров в представление
            ViewBag.Genres = genres;
            ViewBag.Years = years;
            ViewBag.Directors = directors;
            ViewBag.Countries = countries;

            ViewBag.Year = year;
            ViewBag.Country = country;
            ViewBag.Director = director;
            ViewBag.Genre = genre;

            return View(filteredMovies);
        }

    }
}
