using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var services = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllServicesAsync() : await _repository.SearchServicesAsync(q);
        return View(services);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var services = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllServicesAsync() : await _repository.SearchServicesAsync(q);
        return PartialView("_ServicesTable", services);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var services = await _repository.SearchServicesAsync(term);
        var results = services.Select(s => new { id = s.Id, text = s.Name, meta = s.Hotel?.Name });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(new ServiceFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create(ServiceFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
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
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var service = await _repository.GetServiceByIdAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(ServiceFormModel.FromEntity(service));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, ServiceFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
            return View(model);
        }

        var service = await _repository.GetServiceByIdAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        model.UpdateEntity(service);
        _repository.UpdateService(service);
        await _repository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _repository.GetServiceByIdAsync(id);
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteServiceAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var service = await _repository.GetServiceByIdAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        return View(service);
    }
}
