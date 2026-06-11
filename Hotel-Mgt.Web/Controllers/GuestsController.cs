using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelMgt.Web.Controllers;

[Route("gosti")]
public class GuestsController : Controller
{
    private readonly IHotelRepository _repository;

    public GuestsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var guests = string.IsNullOrWhiteSpace(q) ? _repository.GetAllGuests() : _repository.SearchGuests(q);
        return View(guests);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var guests = string.IsNullOrWhiteSpace(q) ? _repository.GetAllGuests() : _repository.SearchGuests(q);
        return PartialView("_GuestsTable", guests);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchGuests(term)
            .Select(g => new { id = g.Id, text = g.FirstName + " " + g.LastName, meta = g.Email });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create()
    {
        return View(new GuestFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create(GuestFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var guest = new Guest
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            DateOfBirth = model.DateOfBirth,
            DocumentNumber = model.DocumentNumber
        };

        _repository.AddGuest(guest);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var guest = _repository.GetGuestById(id);
        if (guest == null)
        {
            return NotFound();
        }

        return View(GuestFormModel.FromEntity(guest));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, GuestFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var guest = _repository.GetGuestById(id);
        if (guest == null)
        {
            return NotFound();
        }

        model.UpdateEntity(guest);
        _repository.UpdateGuest(guest);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var guest = _repository.GetGuestById(id);
        if (guest == null)
        {
            return NotFound();
        }

        return View(guest);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteGuest(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var guest = _repository.GetGuestById(id);
        if (guest == null)
        {
            return NotFound();
        }

        return View(guest);
    }
}
