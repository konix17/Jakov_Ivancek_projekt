using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var guests = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllGuestsAsync() : await _repository.SearchGuestsAsync(q);
        return View(guests);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var guests = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllGuestsAsync() : await _repository.SearchGuestsAsync(q);
        return PartialView("_GuestsTable", guests);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var guests = await _repository.SearchGuestsAsync(term);
        var results = guests.Select(g => new { id = g.Id, text = g.FirstName + " " + g.LastName, meta = g.Email });
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
    public async Task<IActionResult> Create(GuestFormModel model)
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
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var guest = await _repository.GetGuestByIdAsync(id);
        if (guest == null)
        {
            return NotFound();
        }

        return View(GuestFormModel.FromEntity(guest));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, GuestFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var guest = await _repository.GetGuestByIdAsync(id);
        if (guest == null)
        {
            return NotFound();
        }

        model.UpdateEntity(guest);
        _repository.UpdateGuest(guest);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var guest = await _repository.GetGuestByIdAsync(id);
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteGuestAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var guest = await _repository.GetGuestByIdAsync(id);
        if (guest == null)
        {
            return NotFound();
        }

        return View(guest);
    }
}
