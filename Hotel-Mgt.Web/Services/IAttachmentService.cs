using HotelMgt.Model.Entities;
using Microsoft.AspNetCore.Http;

namespace HotelMgt.Web.Services;

public interface IAttachmentService
{
    Task<IEnumerable<Attachment>> GetAttachmentsForHotelAsync(int hotelId);
    Task<Attachment?> UploadAttachmentAsync(int hotelId, IFormFile file);
    Task<bool> DeleteAttachmentAsync(int hotelId, int attachmentId);
}
