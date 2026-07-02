using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMgt.Web.Controllers;

[Route("smjestaji")]
public class RoomsController : Controller
{
    private readonly IHotelRepository _repository;

    public RoomsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var rooms = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllRoomsAsync() : await _repository.SearchRoomsAsync(q);
        return View(rooms);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var rooms = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllRoomsAsync() : await _repository.SearchRoomsAsync(q);
        return PartialView("_RoomsTable", rooms);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var rooms = await _repository.SearchRoomsAsync(term);
        var results = rooms.Select(r => new { id = r.Id, text = r.RoomNumber, meta = r.Hotel?.Name });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(new RoomFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create(RoomFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
            return View(model);
        }

        var room = new Room
        {
            RoomNumber = model.RoomNumber,
            Floor = model.Floor,
            Capacity = model.Capacity,
            PricePerNight = model.PricePerNight,
            RoomType = model.RoomType,
            IsAvailable = model.IsAvailable,
            HotelId = model.HotelId
        };

        _repository.AddRoom(room);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _repository.GetRoomByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(RoomFormModel.FromEntity(room));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, RoomFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
            return View(model);
        }

        var room = await _repository.GetRoomByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        model.UpdateEntity(room);
        _repository.UpdateRoom(room);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _repository.GetRoomByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteRoomAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var room = await _repository.GetRoomByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }
}
