using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var rooms = string.IsNullOrWhiteSpace(q) ? _repository.GetAllRooms() : _repository.SearchRooms(q);
        return View(rooms);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var rooms = string.IsNullOrWhiteSpace(q) ? _repository.GetAllRooms() : _repository.SearchRooms(q);
        return PartialView("_RoomsTable", rooms);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchRooms(term)
            .Select(r => new { id = r.Id, text = r.RoomNumber, meta = r.Hotel?.Name });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create()
    {
        ViewBag.Hotels = _repository.GetAllHotels();
        return View(new RoomFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create(RoomFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = _repository.GetAllHotels();
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
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var room = _repository.GetRoomById(id);
        if (room == null)
        {
            return NotFound();
        }

        ViewBag.Hotels = _repository.GetAllHotels();
        return View(RoomFormModel.FromEntity(room));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, RoomFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = _repository.GetAllHotels();
            return View(model);
        }

        var room = _repository.GetRoomById(id);
        if (room == null)
        {
            return NotFound();
        }

        model.UpdateEntity(room);
        _repository.UpdateRoom(room);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var room = _repository.GetRoomById(id);
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
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteRoom(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var room = _repository.GetRoomById(id);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }
}
