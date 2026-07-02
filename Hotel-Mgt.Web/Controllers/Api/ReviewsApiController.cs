using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/reviews")]
public class ReviewsApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public ReviewsApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllReviewsAsync() : await _repository.SearchReviewsAsync(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReviewDto>> GetReview(int id)
    {
        var entity = await _repository.GetReviewByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ReviewDto>> CreateReview(ReviewCreateDto dto)
    {
        var entity = new Review { Rating = dto.Rating, Comment = dto.Comment, CreatedAt = dto.CreatedAt, GuestId = dto.GuestId, HotelId = dto.HotelId };
        _repository.AddReview(entity);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReview), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateReview(int id, ReviewUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = await _repository.GetReviewByIdAsync(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateReview(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var entity = await _repository.GetReviewByIdAsync(id);
        if (entity == null) return NotFound();
        await _repository.DeleteReviewAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
