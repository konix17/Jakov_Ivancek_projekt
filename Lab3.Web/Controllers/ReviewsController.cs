using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

public class ReviewsController : Controller
{
    private readonly IHotelRepository _repository;

    public ReviewsController(IHotelRepository repository)
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
