using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    [ResponseCache(Duration = 256, Location = ResponseCacheLocation.Any)]
    public class CountriesController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IAllCountries _countriesRepository;

        public CountriesController(IMemoryCache cache, IAllCountries countriesRepository)
        {
            _cache = cache;
            _countriesRepository = countriesRepository;
        }

        public IActionResult Index()
        {
            if (!_cache.TryGetValue("countries", out List<Country> countries))
            {
                countries = _countriesRepository.AllCountries.ToList();
                _cache.Set("countries", countries, TimeSpan.FromSeconds(256));
            }

            return View(countries);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Country country)
        {
            if (ModelState.IsValid)
            {
                _countriesRepository.Create(country);
                return RedirectToAction("Index");
            }
            return View(country);
        }

        public IActionResult Edit(int id)
        {
            var country = _countriesRepository.GetById(id);
            if (country == null) return NotFound();
            return View(country);
        }

        [HttpPost]
        public IActionResult Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                _countriesRepository.Update(country);
                return RedirectToAction("Index");
            }
            return View(country);
        }
    }
}