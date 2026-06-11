using HotelMgt.Model;
using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HotelMgt.Web.Controllers;

[Route("hoteli")]
public class HotelsController : Controller
{
    private readonly IHotelRepository _repository;
    private readonly HotelDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public HotelsController(IHotelRepository repository, HotelDbContext context, IWebHostEnvironment environment)
    {
        _repository = repository;
        _context = context;
        _environment = environment;
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

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create()
    {
        return View(new HotelFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteHotel(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("{id:int}/attachments")]
    public async Task<IActionResult> GetAttachments(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        var attachments = await _context.Attachments
            .Where(a => a.HotelId == id)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return PartialView("_AttachmentList", attachments);
    }

    [HttpPost("{id:int}/attachments/upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        if (file == null || file.Length == 0)
        {
            TempData["AttachmentMessage"] = "No file was selected.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var uploadsPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "hotels", id.ToString());
        Directory.CreateDirectory(uploadsPath);

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
        var fullPath = Path.Combine(uploadsPath, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _context.Attachments.Add(new Attachment
        {
            HotelId = id,
            FileName = Path.GetFileName(file.FileName),
            FilePath = $"/uploads/hotels/{id}/{fileName}",
            ContentType = file.ContentType,
            FileSize = file.Length,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        TempData["AttachmentMessage"] = "The file was uploaded successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:int}/attachments/{attachmentId:int}/delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAttachment(int id, int attachmentId)
    {
        var attachment = await _context.Attachments.SingleOrDefaultAsync(a => a.Id == attachmentId && a.HotelId == id);
        if (attachment == null)
        {
            return NotFound();
        }

        var physicalPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), attachment.FilePath.TrimStart('/'));
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}
