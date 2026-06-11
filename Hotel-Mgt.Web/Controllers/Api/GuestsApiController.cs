using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/guests")]
public class GuestsApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public GuestsApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GuestDto>>> GetGuests([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllGuestsAsync() : await _repository.SearchGuestsAsync(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GuestDto>> GetGuest(int id)
    {
        var entity = await _repository.GetGuestByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<GuestDto>> CreateGuest(GuestCreateDto dto)
    {
        var entity = new Guest { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PhoneNumber = dto.PhoneNumber, DateOfBirth = dto.DateOfBirth, DocumentNumber = dto.DocumentNumber };
        _repository.AddGuest(entity);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGuest), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateGuest(int id, GuestUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = await _repository.GetGuestByIdAsync(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateGuest(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        var entity = await _repository.GetGuestByIdAsync(id);
        if (entity == null) return NotFound();
        await _repository.DeleteGuestAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
