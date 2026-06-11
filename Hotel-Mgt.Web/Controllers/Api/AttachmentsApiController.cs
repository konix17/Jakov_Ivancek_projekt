using HotelMgt.Model;
using HotelMgt.Model.Entities;
using HotelMgt.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/hotels/{hotelId:int}/attachments")]
public class AttachmentsApiController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;

    public AttachmentsApiController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetAttachments(int hotelId)
    {
        var attachments = await _attachmentService.GetAttachmentsForHotelAsync(hotelId);
        return attachments.Select(a => new AttachmentDto(a.Id, a.FileName, a.FilePath, a.ContentType, a.FileSize, a.CreatedAt)).ToList();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAttachment(int hotelId, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var attachment = await _attachmentService.UploadAttachmentAsync(hotelId, file);
        if (attachment == null) return NotFound("Hotel not found or upload failed.");

        return Ok(new { success = true, attachmentId = attachment.Id });
    }

    [HttpDelete("{attachmentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAttachment(int hotelId, int attachmentId)
    {
        var success = await _attachmentService.DeleteAttachmentAsync(hotelId, attachmentId);
        if (!success) return NotFound();

        return NoContent();
    }
}

public record AttachmentDto(int Id, string FileName, string FilePath, string ContentType, long FileSize, DateTime CreatedAt);