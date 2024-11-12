using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Controllers
{
    public class MovieInfoController : Controller
    {
        private readonly Db8428Context _context; // Замените на ваш контекст БД

        public MovieInfoController(Db8428Context context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Genres)
                .Include(m => m.Countries)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
    }
}
