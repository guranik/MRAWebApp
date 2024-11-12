using Microsoft.AspNetCore.Mvc;
using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Controllers
{
    public class GenresController : Controller
    {
        private readonly Db8428Context _context;

        public GenresController(Db8428Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var genres = _context.Genres.ToList();
            return View(genres);
        }
    }
}
