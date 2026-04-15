using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class ReviewsController : Controller
{
    private readonly MockHotelRepository _repository;

    public ReviewsController(MockHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllReviews());
    }

    public IActionResult Details(int id)
    {
        var review = _repository.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }

        return View(review);
    }
}
