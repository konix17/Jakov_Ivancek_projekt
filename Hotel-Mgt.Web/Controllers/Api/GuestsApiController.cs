using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<IEnumerable<GuestDto>> GetGuests([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? _repository.GetAllGuests() : _repository.SearchGuests(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public ActionResult<GuestDto> GetGuest(int id)
    {
        var entity = _repository.GetGuestById(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<GuestDto> CreateGuest(GuestCreateDto dto)
    {
        var entity = new Guest { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PhoneNumber = dto.PhoneNumber, DateOfBirth = dto.DateOfBirth, DocumentNumber = dto.DocumentNumber };
        _repository.AddGuest(entity);
        _repository.SaveChanges();
        return CreatedAtAction(nameof(GetGuest), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult UpdateGuest(int id, GuestUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = _repository.GetGuestById(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateGuest(entity);
        _repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteGuest(int id)
    {
        var entity = _repository.GetGuestById(id);
        if (entity == null) return NotFound();
        _repository.DeleteGuest(id);
        _repository.SaveChanges();
        return NoContent();
    }
}
