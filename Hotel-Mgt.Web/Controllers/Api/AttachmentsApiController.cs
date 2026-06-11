using HotelMgt.Model;
using HotelMgt.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/hotels/{hotelId:int}/attachments")]
public class AttachmentsApiController : ControllerBase
{
    private readonly HotelDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AttachmentsApiController(HotelDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetAttachments(int hotelId)
    {
        var hotel = await _context.Hotels.FindAsync(hotelId);
        if (hotel == null) return NotFound();

        var attachments = await _context.Attachments
            .Where(a => a.HotelId == hotelId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return attachments.Select(a => new AttachmentDto(a.Id, a.FileName, a.FilePath, a.ContentType, a.FileSize, a.CreatedAt)).ToList();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAttachment(int hotelId, IFormFile file)
    {
        var hotel = await _context.Hotels.FindAsync(hotelId);
        if (hotel == null) return NotFound();

        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var uploadsPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "hotels", hotelId.ToString());
        Directory.CreateDirectory(uploadsPath);

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
        var fullPath = Path.Combine(uploadsPath, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var attachment = new Attachment
        {
            HotelId = hotelId,
            FileName = Path.GetFileName(file.FileName),
            FilePath = $"/uploads/hotels/{hotelId}/{fileName}",
            ContentType = file.ContentType,
            FileSize = file.Length,
            CreatedAt = DateTime.UtcNow
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, attachmentId = attachment.Id });
    }

    [HttpDelete("{attachmentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAttachment(int hotelId, int attachmentId)
    {
        var attachment = await _context.Attachments.SingleOrDefaultAsync(a => a.Id == attachmentId && a.HotelId == hotelId);
        if (attachment == null) return NotFound();

        var physicalPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), attachment.FilePath.TrimStart('/'));
        if (System.IO.File.Exists(physicalPath)) System.IO.File.Delete(physicalPath);

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record AttachmentDto(int Id, string FileName, string FilePath, string ContentType, long FileSize, DateTime CreatedAt);