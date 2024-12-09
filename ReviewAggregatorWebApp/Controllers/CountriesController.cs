using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
using ReviewAggregatorWebApp.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    public class CountriesController : Controller
    {
        private readonly IAllCountries _countriesRepository;

        public CountriesController(IAllCountries countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            int pageSize = 20;
            var pagedCountries = _countriesRepository.GetPagedCountries(pageNumber, pageSize);

            ViewBag.CurrentPage = pagedCountries.PageNumber;
            ViewBag.TotalPages = pagedCountries.TotalPages;

            return View(pagedCountries.Items);
        }

        public IActionResult Create()
        {
            return View(new CountryViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var movie = _countriesRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            _countriesRepository.Delete(movie);
            return RedirectToAction("Index");
        }
    }
}