using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReviewAggregatorWebApp.Controllers
{
    public class MovieInfoController : Controller
    {
        private readonly IAllMovies _movieRepository;

        public MovieInfoController(IAllMovies moviesrepository)
        {
            _movieRepository = moviesrepository;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {


            var movie = _movieRepository.GetById(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
    }
}
