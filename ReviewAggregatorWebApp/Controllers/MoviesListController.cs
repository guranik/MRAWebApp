using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.DTOs;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
using ReviewAggregatorWebApp.ViewModel;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    public class MoviesListController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IAllMovies _moviesRepository;
        private readonly IAllCountries _countriesRepository;
        private readonly IAllGenres _genresRepository;
        private readonly IAllDirectors _directorsRepository;
        private readonly IAllYears _yearsRepository;
        

        public MoviesListController(
            IMemoryCache cache, IAllMovies moviesRepository, 
            IAllCountries countriesRepository, IAllGenres genresRepository, 
            IAllDirectors directorsRepository, IAllYears yearsRepository)
        {
            _cache = cache;
            _moviesRepository = moviesRepository;
            _countriesRepository = countriesRepository;
            _genresRepository = genresRepository;
            _directorsRepository = directorsRepository;
            _yearsRepository = yearsRepository;
        }

        public IActionResult Filter(string genre = "", string year = "", string director = "", string country = "", string sortBy = "rating", int pageNumber = 1)
        {
            if(!_cache.TryGetValue("genres", out var genres))
            {
                throw new Exception("Жанры не найдены в кэше.");
            }
            if (!_cache.TryGetValue("years", out var years))
            {
                throw new Exception("Годы не найдены в кэше.");
            }
            if(!_cache.TryGetValue("countries", out var countries))
            {
                throw new Exception("Страны не найдены в кэше.");
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                HttpContext.Session.SetString("SortOrder", sortBy);
            }
            else
            {
                sortBy = HttpContext.Session.GetString("SortOrder") ?? "rating";
            }

            HttpContext.Session.SetString("FilterGenre", genre);
            HttpContext.Session.SetString("FilterYear", year);
            HttpContext.Session.SetString("FilterDirector", director);
            HttpContext.Session.SetString("FilterCountry", country);
            HttpContext.Session.SetInt32("CurrentPage", pageNumber);

            var filteredMovies = _moviesRepository.GetPagedMovies(genre, year, director, country, sortBy, pageNumber, 15);  

            // Передаем данные для фильтров в представление
            ViewBag.Genres = genres;
            ViewBag.Years = years;
            ViewBag.Countries = countries;

            ViewBag.Year = year;
            ViewBag.Country = country;
            ViewBag.Director = director;
            ViewBag.Genre = genre;
            ViewBag.SortOrder = sortBy;
            ViewBag.CurrentPage = filteredMovies.PageNumber;
            ViewBag.TotalPages = filteredMovies.TotalPages;

            return View(filteredMovies.Items);
        }

        public IActionResult Create()
        {
            var viewModel = new MovieViewModel
            {
                Directors = new SelectList(_directorsRepository.AllDirectors, "Id", "Name"),
                Genres = new SelectList(_genresRepository.AllGenres, "Id", "Name"),
                Countries = new SelectList(_countriesRepository.AllCountries, "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                var movie = new Movie
                {
                    Name = model.Name,
                    PosterLink = model.PosterLink,
                    DirectorId = model.DirectorId,
                    ReleaseDate = model.ReleaseDate,
                    Rating = null,
                    Countries = model.CountryIds.Select(id => _countriesRepository.GetById(id)).ToList(),
                    Genres = model.GenreIds.Select(id => _genresRepository.GetById(id)).ToList()
                };

                if (!movie.Countries.Any() || !movie.Genres.Any())
                {
                    ModelState.AddModelError("", "Фильм должен иметь хотя бы одну страну и один жанр.");
                }
                else
                {
                    _moviesRepository.Create(movie);
                    return RedirectWithSession();
                }
            }

            // Заполнение списков в случае ошибки валидации
            model.Directors = new SelectList(_directorsRepository.AllDirectors, "Id", "Name");
            model.Genres = new SelectList(_genresRepository.AllGenres, "Id", "Name");
            model.Countries = new SelectList(_countriesRepository.AllCountries, "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public IActionResult GetMoviesByTitlePrefix(string titlePrefix)
        {
            if (string.IsNullOrWhiteSpace(titlePrefix))
            {
                return BadRequest("Title prefix cannot be empty.");
            }

            var movies = _moviesRepository.GetMoviesByTitlePrefix(titlePrefix);

            var movieDtos = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Name = m.Name
            });

            return Json(movieDtos);
        }

        public IActionResult Edit(int id)
        {
            var movie = _moviesRepository.GetById(id);
            if (movie == null) return NotFound();

            var model = new MovieViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                PosterLink = movie.PosterLink,
                DirectorId = movie.DirectorId,
                Director = movie.Director,
                ReleaseDate = movie.ReleaseDate,
                CountryIds = movie.Countries.Select(c => c.Id).ToList(),
                GenreIds = movie.Genres.Select(g => g.Id).ToList(),
                Directors = new SelectList(_directorsRepository.AllDirectors, "Id", "Name"),
                Genres = new SelectList(_genresRepository.AllGenres, "Id", "Name"),
                Countries = new SelectList(_countriesRepository.AllCountries, "Id", "Name"),
                IsEditing = true
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingMovie = _moviesRepository.GetById(model.Id);
                if (existingMovie == null) return NotFound();

                existingMovie.Name = model.Name;
                existingMovie.PosterLink = model.PosterLink;
                existingMovie.DirectorId = model.DirectorId;
                existingMovie.ReleaseDate = model.ReleaseDate;

                existingMovie.Countries = model.CountryIds.Select(id => _countriesRepository.GetById(id)).ToList();
                existingMovie.Genres = model.GenreIds.Select(id => _genresRepository.GetById(id)).ToList();

                if (!existingMovie.Countries.Any() || !existingMovie.Genres.Any())
                {
                    ModelState.AddModelError("", "Фильм должен иметь хотя бы одну страну и один жанр.");
                }
                else
                {
                    _moviesRepository.Update(existingMovie);
                    return RedirectWithSession();
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                // Логирование или вывод ошибок
                Console.WriteLine(error.ErrorMessage);
            }

            // Заполнение списков в случае ошибки валидации
            model.Directors = new SelectList(_directorsRepository.AllDirectors, "Id", "Name");
            model.Genres = new SelectList(_genresRepository.AllGenres, "Id", "Name");
            model.Countries = new SelectList(_countriesRepository.AllCountries, "Id", "Name");
            model.IsEditing = true;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var movie = _moviesRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            _moviesRepository.Delete(movie);
            return RedirectWithSession();
        }

        private IActionResult RedirectWithSession()
        {
            var genre = HttpContext.Session.GetString("FilterGenre") ?? "";
            var year = HttpContext.Session.GetString("FilterYear") ?? "";
            var director = HttpContext.Session.GetString("FilterDirector") ?? "";
            var country = HttpContext.Session.GetString("FilterCountry") ?? "";
            var sortBy = HttpContext.Session.GetString("SortOrder") ?? "rating";
            var pageNumber = HttpContext.Session.GetInt32("CurrentPage") ?? 1;

            return RedirectToAction("Filter", new { genre, year, director, country, sortBy, pageNumber });
        }
    }
}
