using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.DTOs;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
using ReviewAggregatorWebApp.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    public class DirectorsController : Controller
    {
        private readonly IAllDirectors _directorsRepository;

        public DirectorsController(IAllDirectors directorsRepository)
        {
            _directorsRepository = directorsRepository;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            int pageSize = 20;
            var pagedDirectors = _directorsRepository.GetPagedDirectors(pageNumber, pageSize);

            ViewBag.CurrentPage = pagedDirectors.PageNumber;
            ViewBag.TotalPages = pagedDirectors.TotalPages;

            return View(pagedDirectors.Items);
        }

        [HttpGet]
        public IActionResult GetDirectorsByNamePrefix(string namePrefix)
        {
            if (string.IsNullOrWhiteSpace(namePrefix))
            {
                return BadRequest("Name prefix cannot be empty.");
            }

            var directors = _directorsRepository.GetDirectorsByNamePrefix(namePrefix);

            var directorDtos = directors.Select(d => new DirectorDto
            {
                Name = d.Name
            });

            return Json(directorDtos);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new DirectorViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var movie = _directorsRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            _directorsRepository.Delete(movie);
            return RedirectToAction("Index");
        }
    }
}