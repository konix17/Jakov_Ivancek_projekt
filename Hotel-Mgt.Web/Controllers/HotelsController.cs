using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelMgt.Web.Controllers;

[Route("hoteli")]
public class HotelsController : Controller
{
    private readonly IHotelRepository _repository;

    public HotelsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var hotels = string.IsNullOrWhiteSpace(q) ? _repository.GetAllHotels() : _repository.SearchHotels(q);
        return View(hotels);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var hotels = string.IsNullOrWhiteSpace(q) ? _repository.GetAllHotels() : _repository.SearchHotels(q);
        return PartialView("_HotelsTable", hotels);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchHotels(term)
            .Select(h => new { id = h.Id, text = h.Name, meta = h.City });
        return Json(results);
    }

    [Route("create")]
    public IActionResult Create()
    {
        return View(new HotelFormModel());
    }

    [HttpPost]
    [Route("create")]
    public IActionResult Create(HotelFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var hotel = new Hotel
        {
            Name = model.Name,
            Address = model.Address,
            City = model.City,
            Rating = model.Rating,
            PhoneNumber = model.PhoneNumber
        };

        _repository.AddHotel(hotel);
        _repository.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var hotel = _repository.GetHotelById(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return View(HotelFormModel.FromEntity(hotel));
    }

    [HttpPost]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, HotelFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var hotel = _repository.GetHotelById(id);
        if (hotel == null)
        {
            return NotFound();
        }

        model.UpdateEntity(hotel);
        _repository.UpdateHotel(hotel);
        _repository.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var hotel = _repository.GetHotelById(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return View(hotel);
    }

    [HttpPost]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteHotel(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var hotel = _repository.GetHotelById(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return View(hotel);
    }
}
