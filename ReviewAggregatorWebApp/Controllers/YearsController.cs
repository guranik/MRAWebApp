using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Repository;
using System.Collections.Generic;

namespace ReviewAggregatorWebApp.Controllers
{
    public class YearsController : Controller
    {
        private readonly IAllYears _yearRepository;

        public YearsController(IAllYears yearRepository)
        {
            _yearRepository = yearRepository;
        }

        public IActionResult Index()
        {
            var years = _yearRepository.AllYears.ToList();

            return View(years);
        }
    }
}