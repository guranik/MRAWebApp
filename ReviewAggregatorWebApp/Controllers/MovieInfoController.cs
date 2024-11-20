using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    public class MovieInfoController : Controller
    {
        private readonly IMemoryCache _cache;

        public MovieInfoController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Details(int id)
        {
            if (!_cache.TryGetValue("movies", out List<Movie> movies))
            {
                return NotFound("Movies not found in cache.");
            }

            var movie = movies
                .Where(m => m.Id == id)
                .Select(m => new Movie
                {
                    Id = m.Id,
                    Name = m.Name,
                    ReleaseDate = m.ReleaseDate,
                    Director = m.Director,
                    Genres = m.Genres,
                    Countries = m.Countries,
                    Rating = m.Rating,
                    PosterLink = m.PosterLink
                })
                .FirstOrDefault();

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
    }
}
