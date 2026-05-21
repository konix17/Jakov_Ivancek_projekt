using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelMgt.Web.Controllers;

[Route("recenzije")]
public class ReviewsController : Controller
{
    private readonly IHotelRepository _repository;

    public ReviewsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var reviews = string.IsNullOrWhiteSpace(q) ? _repository.GetAllReviews() : _repository.SearchReviews(q);
        return View(reviews);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var reviews = string.IsNullOrWhiteSpace(q) ? _repository.GetAllReviews() : _repository.SearchReviews(q);
        return PartialView("_ReviewsTable", reviews);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchReviews(term)
            .Select(r => new { id = r.Id, text = r.Comment, meta = r.Guest?.FirstName + " " + r.Guest?.LastName });
        return Json(results);
    }

    [Route("create")]
    public IActionResult Create()
    {
        ViewBag.Guests = _repository.GetAllGuests();
        ViewBag.Hotels = _repository.GetAllHotels();
        return View(new ReviewFormModel());
    }

    [HttpPost]
    [Route("create")]
    public IActionResult Create(ReviewFormModel model)
    {
        model.CreatedAt = DateTime.Today;

        if (!ModelState.IsValid)
        {
            ViewBag.Guests = _repository.GetAllGuests();
            ViewBag.Hotels = _repository.GetAllHotels();
            return View(model);
        }

        var review = new Review
        {
            Rating = model.Rating,
            Comment = model.Comment,
            CreatedAt = model.CreatedAt,
            GuestId = model.GuestId,
            HotelId = model.HotelId
        };

        _repository.AddReview(review);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var review = _repository.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }

        ViewBag.Guests = _repository.GetAllGuests();
        ViewBag.Hotels = _repository.GetAllHotels();
        return View(ReviewFormModel.FromEntity(review));
    }

    [HttpPost]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, ReviewFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Guests = _repository.GetAllGuests();
            ViewBag.Hotels = _repository.GetAllHotels();
            return View(model);
        }

        var review = _repository.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }

        model.UpdateEntity(review);
        _repository.UpdateReview(review);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var review = _repository.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }

        return View(review);
    }

    [HttpPost]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteReview(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("{id:int}")]
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
