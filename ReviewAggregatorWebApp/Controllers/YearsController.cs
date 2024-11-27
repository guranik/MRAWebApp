using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Repository;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Controllers
{
    [ResponseCache(Duration = 256, Location = ResponseCacheLocation.Any)]
    public class YearsController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IAllYears _yearRepository;

        public YearsController(IMemoryCache cache, IAllYears yearRepository)
        {
            _cache = cache;
            _yearRepository = yearRepository;
        }

        public IActionResult Index()
        {
            if (!_cache.TryGetValue("years", out List<int> years))
            {
                years = _yearRepository.AllYears.ToList();
                _cache.Set("years", years, TimeSpan.FromSeconds(256));
            }

            return View(years);
        }
    }
}