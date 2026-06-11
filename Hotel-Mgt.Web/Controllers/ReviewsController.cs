using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;

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
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var reviews = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllReviewsAsync() : await _repository.SearchReviewsAsync(q);
        return View(reviews);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var reviews = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllReviewsAsync() : await _repository.SearchReviewsAsync(q);
        return PartialView("_ReviewsTable", reviews);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var reviews = await _repository.SearchReviewsAsync(term);
        var results = reviews.Select(r => new { id = r.Id, text = r.Comment, meta = r.Guest?.FirstName + " " + r.Guest?.LastName });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Guests = await _repository.GetAllGuestsAsync();
        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(new ReviewFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create(ReviewFormModel model)
    {
        model.CreatedAt = DateTime.Today;

        if (!ModelState.IsValid)
        {
            ViewBag.Guests = await _repository.GetAllGuestsAsync();
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
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
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var review = await _repository.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        ViewBag.Guests = await _repository.GetAllGuestsAsync();
        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(ReviewFormModel.FromEntity(review));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, ReviewFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Guests = await _repository.GetAllGuestsAsync();
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
            return View(model);
        }

        var review = await _repository.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        model.UpdateEntity(review);
        _repository.UpdateReview(review);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _repository.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        return View(review);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteReviewAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var review = await _repository.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        return View(review);
    }
}
