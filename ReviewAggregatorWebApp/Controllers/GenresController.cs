using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Controllers
{
    [ResponseCache(Duration = 256, Location = ResponseCacheLocation.Any)]
    public class GenresController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IAllGenres _genresRepository;

        public GenresController(IMemoryCache cache, IAllGenres genresRepository)
        {
            _cache = cache;
            _genresRepository = genresRepository;
        }

        public IActionResult Index()
        {
            if (!_cache.TryGetValue("genres", out List<Genre> genres))
            {
                genres = _genresRepository.AllGenres.ToList();

                _cache.Set("genres", genres, TimeSpan.FromSeconds(256));
            }

            return View(genres);
        }
    }
}
