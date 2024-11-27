using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    [ResponseCache(Duration = 256, Location = ResponseCacheLocation.Any)]
    public class DirectorsController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IAllDirectors _directorsRepository;

        public DirectorsController(IMemoryCache cache, IAllDirectors directorsRepository)
        {
            _cache = cache;
            _directorsRepository = directorsRepository;
        }

        public IActionResult Index()
        {
            if (!_cache.TryGetValue("directors", out List<Director> directors))
            {
                directors = _directorsRepository.AllDirectors.ToList();
                _cache.Set("directors", directors, TimeSpan.FromSeconds(256));
            }

            return View(directors);
        }
    }
}