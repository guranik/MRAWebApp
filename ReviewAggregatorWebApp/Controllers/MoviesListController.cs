using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
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

        public MoviesListController(
            IMemoryCache cache, IAllMovies moviesRepository, 
            IAllCountries countriesRepository, IAllGenres genresRepository, 
            IAllDirectors directorsRepository)
        {
            _cache = cache;
            _moviesRepository = moviesRepository;
            _countriesRepository = countriesRepository;
            _genresRepository = genresRepository;
            _directorsRepository = directorsRepository;
        }

        public IActionResult Filter(string genre = "", string year = "", string director = "", string country = "", string sortBy = "rating", int pageNumber = 1)
        {
            if (!_cache.TryGetValue("genres", out List<Genre>
            genres))
            {
                return NotFound("Genres not found in cache.");
            }
            if (!_cache.TryGetValue("countries", out List<Country>
            countries))
            {
                return NotFound("Countries not found in cache.");
            }
            if (!_cache.TryGetValue("directors", out List<Director>
            directors))
            {
                return NotFound("Directors not found in cache.");
            }
            if (!_cache.TryGetValue("years", out List<int>
            years))
            {
                return NotFound("Years not found in cache.");
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                HttpContext.Session.SetString("SortOrder", sortBy);
            }
            else
            {
                sortBy = HttpContext.Session.GetString("SortOrder") ?? "rating";
            }

            var filteredMovies = _moviesRepository.GetPagedMovies(genre, year, director, country, sortBy, pageNumber, 15);  


            // Передаем данные для фильтров в представление
            ViewBag.Genres = genres;
            ViewBag.Years = years;
            ViewBag.Directors = directors;
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
                    Rating = model.Rating,
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
                    return RedirectToAction("Filter");
                }
            }

            // Заполнение списков в случае ошибки валидации
            model.Directors = new SelectList(_directorsRepository.AllDirectors, "Id", "Name");
            model.Genres = new SelectList(_genresRepository.AllGenres, "Id", "Name");
            model.Countries = new SelectList(_countriesRepository.AllCountries, "Id", "Name");
            return View(model);
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
                ReleaseDate = movie.ReleaseDate,
                Rating = movie.Rating,
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
                existingMovie.Rating = model.Rating;

                existingMovie.Countries = model.CountryIds.Select(id => _countriesRepository.GetById(id)).ToList();
                existingMovie.Genres = model.GenreIds.Select(id => _genresRepository.GetById(id)).ToList();

                if (!existingMovie.Countries.Any() || !existingMovie.Genres.Any())
                {
                    ModelState.AddModelError("", "Фильм должен иметь хотя бы одну страну и один жанр.");
                }
                else
                {
                    _moviesRepository.Update(existingMovie);
                    return RedirectToAction("Filter");
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

        public IActionResult Delete(int id)
        {
            var movie = _moviesRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            _moviesRepository.Delete(movie);
            return RedirectToAction("Index");
        }
    }
}
