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
    public class DirectorsController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IAllDirectors _directorsRepository;

        public DirectorsController(IMemoryCache cache, IAllDirectors directorsRepository)
        {
            _cache = cache;
            _directorsRepository = directorsRepository;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            int pageSize = 20;
            var pagedDirectors = _directorsRepository.GetPagedDirectors(pageNumber, pageSize);

            if (!_cache.TryGetValue("directors", out List<Director> directors))
            {
                directors = pagedDirectors.Items;
                _cache.Set("directors", directors, TimeSpan.FromSeconds(256));
            }

            ViewBag.CurrentPage = pagedDirectors.PageNumber;
            ViewBag.TotalPages = pagedDirectors.TotalPages;

            return View(pagedDirectors.Items);
        }

        public IActionResult Create()
        {
            return View(new DirectorViewModel());
        }

        [HttpPost]
        public IActionResult Create(DirectorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var director = new Director { Name = model.Name };
                _directorsRepository.Create(director);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var director = _directorsRepository.GetById(id);
            if (director == null) return NotFound();

            var model = new DirectorViewModel { Id = director.Id, Name = director.Name, IsEditing = true };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(DirectorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var director = _directorsRepository.GetById(model.Id);
                if (director == null) return NotFound();

                director.Name = model.Name;
                _directorsRepository.Update(director);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}