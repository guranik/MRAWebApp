using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
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

        public IActionResult Filter(string genre = "", string year = "", string director = "", string country = "", string sortBy = "rating")
        {
            if (!_cache.TryGetValue("movies", out List<Movie>
            movies))
            {
                return NotFound("Movies not found in cache.");
            }
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

            ViewBag.SortOrder = sortBy;

            IEnumerable<Movie>
                filteredMovies = movies;

            if (!string.IsNullOrEmpty(genre))
            {
                filteredMovies = filteredMovies.Where(m => m.Genres.Any(g => g.Name == genre));
            }

            if (!string.IsNullOrEmpty(director))
            {
                filteredMovies = filteredMovies.Where(m => m.Director != null && m.Director.Name == director);
            }

            if (!string.IsNullOrEmpty(country))
            {
                filteredMovies = filteredMovies.Where(m => m.Countries.Any(c => c.Name == country));
            }

            if (!string.IsNullOrEmpty(year))
            {
                filteredMovies = filteredMovies.Where(m => m.ReleaseDate.Year.ToString() == year);
            }

            switch (sortBy?.ToLower())
            {
                case "rating":
                    filteredMovies = filteredMovies.OrderByDescending(m => m.Rating);
                    break;
                case "date":
                    filteredMovies = filteredMovies.OrderByDescending(m => m.ReleaseDate);
                    break;
            }

            // Передаем данные для фильтров в представление
            ViewBag.Genres = genres;
            ViewBag.Years = years;
            ViewBag.Directors = directors;
            ViewBag.Countries = countries;

            ViewBag.Year = year;
            ViewBag.Country = country;
            ViewBag.Director = director;
            ViewBag.Genre = genre;

            return View(filteredMovies);
        }

        public IActionResult Create()
        {
            ViewBag.Countries = _countriesRepository.AllCountries.ToList();
            ViewBag.Genres = _genresRepository.AllGenres.ToList();
            ViewBag.Directors = _directorsRepository.AllDirectors.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Movie movie, int[] selectedCountries, int[] selectedGenres)
        {
            if (ModelState.IsValid)
            {
                // Добавление выбранных стран и жанров
                if (selectedCountries != null)
                {
                    movie.Countries = selectedCountries.Select(id => _countriesRepository.GetById(id)).ToList();
                }
                if (selectedGenres != null)
                {
                    movie.Genres = selectedGenres.Select(id => _genresRepository.GetById(id)).ToList();
                }

                if (!movie.Countries.Any() || !movie.Genres.Any())
                {
                    ModelState.AddModelError("", "Фильм должен иметь хотя бы одну страну и один жанр.");
                    ViewBag.Countries = _countriesRepository.AllCountries.ToList();
                    ViewBag.Genres = _genresRepository.AllGenres.ToList();
                    ViewBag.Directors = _directorsRepository.AllDirectors.ToList();
                    return View(movie);
                }

                _moviesRepository.Create(movie);
                return RedirectToAction("Index");
            }

            ViewBag.Countries = _countriesRepository.AllCountries.ToList();
            ViewBag.Genres = _genresRepository.AllGenres.ToList();
            ViewBag.Directors = _directorsRepository.AllDirectors.ToList();
            return View(movie);
        }

        public IActionResult Edit(int id)
        {
            var movie = _moviesRepository.GetById(id);
            if (movie == null) return NotFound();

            ViewBag.Countries = _countriesRepository.AllCountries.ToList();
            ViewBag.Genres = _genresRepository.AllGenres.ToList();
            ViewBag.Directors = _directorsRepository.AllDirectors.ToList();
            return View(movie);
        }

        [HttpPost]
        public IActionResult Edit(Movie movie, int[] selectedCountries, int[] selectedGenres)
        {
            if (ModelState.IsValid)
            {
                // Получаем существующий фильм из базы данных
                var existingMovie = _moviesRepository.GetById(movie.Id);

                // Обновляем свойства
                existingMovie.Name = movie.Name;
                existingMovie.PosterLink = movie.PosterLink;
                existingMovie.DirectorId = movie.DirectorId;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.Rating = movie.Rating;

                // Обновление выбранных стран
                if (selectedCountries != null)
                {
                    existingMovie.Countries = selectedCountries.Select(id => _countriesRepository.GetById(id)).ToList();
                }
                else
                {
                    existingMovie.Countries.Clear(); // Если страны не выбраны, очищаем
                }

                // Обновление выбранных жанров
                if (selectedGenres != null)
                {
                    existingMovie.Genres = selectedGenres.Select(id => _genresRepository.GetById(id)).ToList();
                }
                else
                {
                    existingMovie.Genres.Clear(); // Если жанры не выбраны, очищаем
                }

                if (!existingMovie.Countries.Any() || !existingMovie.Genres.Any())
                {
                    ModelState.AddModelError("", "Фильм должен иметь хотя бы одну страну и один жанр.");
                    ViewBag.Countries = _countriesRepository.AllCountries.ToList();
                    ViewBag.Genres = _genresRepository.AllGenres.ToList();
                    ViewBag.Directors = _directorsRepository.AllDirectors.ToList();
                    return View(movie);
                }

                _moviesRepository.Update(existingMovie);
                return RedirectToAction("Index");
            }

            ViewBag.Countries = _countriesRepository.AllCountries.ToList();
            ViewBag.Genres = _genresRepository.AllGenres.ToList();
            ViewBag.Directors = _directorsRepository.AllDirectors.ToList();
            return View(movie);
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
