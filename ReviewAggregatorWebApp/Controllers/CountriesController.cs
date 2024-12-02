using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.ViewModel;
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
            return View(new CountryViewModel());
        }

        [HttpPost]
        public IActionResult Create(CountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var country = new Country { Name = model.Name };
                _countriesRepository.Create(country);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var country = _countriesRepository.GetById(id);
            if (country == null) return NotFound();

            var model = new CountryViewModel { Id = country.Id, Name = country.Name, IsEditing = true };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var country = _countriesRepository.GetById(model.Id);
                if (country == null) return NotFound();

                country.Name = model.Name;
                _countriesRepository.Update(country);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}