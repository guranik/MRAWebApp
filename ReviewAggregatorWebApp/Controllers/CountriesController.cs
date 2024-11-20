using Microsoft.AspNetCore.Mvc;
using ReviewAggregatorWebApp.Model;

namespace ReviewAggregatorWebApp.Controllers
{
    public class CountriesController : Controller
    {
        private readonly Db8428Context _context;

        public CountriesController(Db8428Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var countries = _context.Countries.ToList();
            return View(countries);
        }
    }
}
