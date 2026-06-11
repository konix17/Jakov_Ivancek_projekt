using HotelMgt.Model;
using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HotelMgt.Web.Controllers;

[Route("hoteli")]
public class HotelsController : Controller
{
    private readonly IHotelRepository _repository;
    private readonly HotelMgt.Web.Services.IAttachmentService _attachmentService;

    public HotelsController(IHotelRepository repository, HotelMgt.Web.Services.IAttachmentService attachmentService)
    {
        _repository = repository;
        _attachmentService = attachmentService;
    }

    [Route("")]
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var hotels = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllHotelsAsync() : await _repository.SearchHotelsAsync(q);
        return View(hotels);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var hotels = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllHotelsAsync() : await _repository.SearchHotelsAsync(q);
        return PartialView("_HotelsTable", hotels);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var hotels = await _repository.SearchHotelsAsync(term);
        var results = hotels.Select(h => new { id = h.Id, text = h.Name, meta = h.City });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create()
    {
        return View(new HotelFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create(HotelFormModel model)
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
        await _repository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var hotel = await _repository.GetHotelByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return View(HotelFormModel.FromEntity(hotel));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, HotelFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var hotel = await _repository.GetHotelByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        model.UpdateEntity(hotel);
        _repository.UpdateHotel(hotel);
        await _repository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hotel = await _repository.GetHotelByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return View(hotel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteHotelAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var hotel = await _repository.GetHotelByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return View(hotel);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/attachments")]
    public async Task<IActionResult> GetAttachments(int id, [FromQuery] bool isEdit = false)
    {
        var hotel = await _repository.GetHotelByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        var attachments = await _attachmentService.GetAttachmentsForHotelAsync(id);
        ViewData["IsEdit"] = isEdit;
        return PartialView("_AttachmentList", attachments);
    }

    [HttpPost("{id:int}/attachments/upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
    {
        var hotel = await _repository.GetHotelByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        if (file == null || file.Length == 0)
        {
            TempData["AttachmentMessage"] = "No file was selected.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var attachment = await _attachmentService.UploadAttachmentAsync(id, file);
        if (attachment != null)
        {
            TempData["AttachmentMessage"] = "The file was uploaded successfully.";
        }
        else
        {
            TempData["AttachmentMessage"] = "The file upload failed.";
        }
        
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:int}/attachments/{attachmentId:int}/delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAttachment(int id, int attachmentId)
    {
        var success = await _attachmentService.DeleteAttachmentAsync(id, attachmentId);
        if (!success)
        {
            return NotFound();
        }

        return Json(new { success = true });
    }
}
