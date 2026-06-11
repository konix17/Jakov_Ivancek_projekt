using HotelMgt.Model;
using HotelMgt.Model.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMgt.Web.Services;

public class AttachmentService : IAttachmentService
{
    private readonly HotelDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AttachmentService(HotelDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IEnumerable<Attachment>> GetAttachmentsForHotelAsync(int hotelId)
    {
        var hotel = await _context.Hotels.FindAsync(hotelId);
        if (hotel == null) return Enumerable.Empty<Attachment>();

        return await _context.Attachments
            .Where(a => a.HotelId == hotelId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Attachment?> UploadAttachmentAsync(int hotelId, IFormFile file)
    {
        var hotel = await _context.Hotels.FindAsync(hotelId);
        if (hotel == null) return null;

        if (file == null || file.Length == 0) return null;

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

        return attachment;
    }

    public async Task<bool> DeleteAttachmentAsync(int hotelId, int attachmentId)
    {
        var attachment = await _context.Attachments.SingleOrDefaultAsync(a => a.Id == attachmentId && a.HotelId == hotelId);
        if (attachment == null) return false;

        var physicalPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), attachment.FilePath.TrimStart('/'));
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return true;
    }
}
