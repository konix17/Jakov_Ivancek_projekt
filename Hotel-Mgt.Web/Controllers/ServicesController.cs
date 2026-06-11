using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelMgt.Web.Controllers;

[Route("usluge")]
public class ServicesController : Controller
{
    private readonly IHotelRepository _repository;

    public ServicesController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var services = string.IsNullOrWhiteSpace(q) ? _repository.GetAllServices() : _repository.SearchServices(q);
        return View(services);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var services = string.IsNullOrWhiteSpace(q) ? _repository.GetAllServices() : _repository.SearchServices(q);
        return PartialView("_ServicesTable", services);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchServices(term)
            .Select(s => new { id = s.Id, text = s.Name, meta = s.Hotel?.Name });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create()
    {
        ViewBag.Hotels = _repository.GetAllHotels();
        return View(new ServiceFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create(ServiceFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = _repository.GetAllHotels();
            return View(model);
        }

        var service = new Service
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            IsAvailable = model.IsAvailable,
            HotelId = model.HotelId
        };

        _repository.AddService(service);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var service = _repository.GetServiceById(id);
        if (service == null)
        {
            return NotFound();
        }

        ViewBag.Hotels = _repository.GetAllHotels();
        return View(ServiceFormModel.FromEntity(service));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, ServiceFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = _repository.GetAllHotels();
            return View(model);
        }

        var service = _repository.GetServiceById(id);
        if (service == null)
        {
            return NotFound();
        }

        model.UpdateEntity(service);
        _repository.UpdateService(service);
        _repository.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var service = _repository.GetServiceById(id);
        if (service == null)
        {
            return NotFound();
        }

        return View(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteService(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var service = _repository.GetServiceById(id);
        if (service == null)
        {
            return NotFound();
        }

        return View(service);
    }
}
