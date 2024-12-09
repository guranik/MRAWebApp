using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
using ReviewAggregatorWebApp.ViewModel;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Controllers
{
    public class GenresController : Controller
    {
        private readonly IAllGenres _genresRepository;

        public GenresController(IAllGenres genresRepository)
        {
            _genresRepository = genresRepository;
        }

        public IActionResult Index()
        {
            var genres = _genresRepository.AllGenres.ToList();

            return View(genres);
        }

        public IActionResult Create()
        {
            return View(new GenreViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var movie = _genresRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            _genresRepository.Delete(movie);
            return RedirectToAction("Index");
        }
    }
}
