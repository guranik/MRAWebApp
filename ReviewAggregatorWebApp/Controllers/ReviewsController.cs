using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System.Security.Claims;

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

    public IActionResult Create([FromBody] Review review)
    {
        if (ModelState.IsValid)
        {
            // Получаем ID пользователя из claims и конвертируем его в int
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out int userId))
            {
                review.UserId = userId; // Устанавливаем ID пользователя
                _reviewRepository.Create(review);
                return Json(new { success = true });
            }
            else
            {
                // Обработка ошибки, если ID пользователя не является корректным числом
                return Json(new { success = false, message = "Invalid user ID." });
            }
        }
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in errors)
        {
            // Логирование или вывод ошибок
            Console.WriteLine(error.ErrorMessage);
        }
        return Json(new { success = false });
    }
}