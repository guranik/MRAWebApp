using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
using ReviewAggregatorWebApp.ViewModel;
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

        public IActionResult Create()
        {
            return View(new GenreViewModel());
        }

        [HttpPost]
        public IActionResult Create(GenreViewModel model)
        {
            if (ModelState.IsValid)
            {
                var genre = new Genre { Name = model.Name };
                _genresRepository.Create(genre);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var genre = _genresRepository.GetById(id);
            if (genre == null) return NotFound();

            var model = new GenreViewModel { Id = genre.Id, Name = genre.Name, IsEditing = true };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(GenreViewModel model)
        {
            if (ModelState.IsValid)
            {
                var genre = _genresRepository.GetById(model.Id);
                if (genre == null) return NotFound();

                genre.Name = model.Name;
                _genresRepository.Update(genre);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
