using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAggregatorWebApp.BusinessLogic;
using ReviewAggregatorWebApp.DTOs;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Interfaces.Services;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.Repository;
using System.Security.Claims;

public class ReviewsController : Controller
{
    private readonly IAllReviews _reviewRepository;
    private readonly IAllMovies _movieRepository;

    public ReviewsController(IAllReviews reviewRepository, IAllMovies movieRepository)
    {
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
    }

    [HttpGet]
    public IActionResult GetReviews(int movieId, int page = 1)
    {
        var reviews = _reviewRepository.GetPagedReviews(movieId, page, 10);

        var reviewDtos = reviews.Items.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User.Login,
            MovieId = r.MovieId,
            PostDate = r.PostDate,
            Rating = r.Rating,
            ReviewText = r.ReviewText
        }).ToList();

        return Json(new { reviews = reviewDtos, reviews.PageNumber, reviews.TotalPages });
    }

    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] Review review)
    {
        if (ModelState.IsValid)
        {
            // Получаем ID пользователя из claims и конвертируем его в int
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out int userId))
            {
                review.UserId = userId; // Устанавливаем ID пользователя
                review.PostDate = DateTime.Now;
                _reviewRepository.Create(review);

                IRatingService ratingService = new RatingLogic(_reviewRepository, _movieRepository);
                ratingService.CalculateRating(review.MovieId);
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

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var review = _reviewRepository.GetById(id);
        if (review == null)
        {
            return NotFound();
        }

        _reviewRepository.Delete(review);
        return RedirectToAction("Details", "MovieInfo", new { id = review.MovieId });
    }
}