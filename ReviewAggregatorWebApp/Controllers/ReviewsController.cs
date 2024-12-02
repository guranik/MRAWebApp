using Microsoft.AspNetCore.Mvc;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;

public class ReviewsController : Controller
{
    private readonly IAllReviews _reviewRepository;

    public ReviewsController(IAllReviews reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    [HttpGet]
    public IActionResult GetReviews(int movieId, int page = 1, int pageSize = 10)
    {
        var reviews = _reviewRepository.GetByMovieId(movieId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalReviews = _reviewRepository.GetByMovieId(movieId).Count();
        var totalPages = (int)Math.Ceiling((double)totalReviews / pageSize);

        return Json(new { reviews, totalPages });
    }

    [HttpPost]
    public IActionResult Create([FromBody] Review review)
    {
        if (ModelState.IsValid)
        {
            _reviewRepository.Create(review);
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }
}