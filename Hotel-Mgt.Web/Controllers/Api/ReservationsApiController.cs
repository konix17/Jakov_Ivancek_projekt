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
[Route("api/reservations")]
public class ReservationsApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public ReservationsApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllReservationsAsync() : await _repository.SearchReservationsAsync(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(int id)
    {
        var entity = await _repository.GetReservationByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ReservationDto>> CreateReservation(ReservationCreateDto dto)
    {
        var entity = new Reservation { ReservationCode = dto.ReservationCode, ReservationDate = dto.ReservationDate, CheckInDate = dto.CheckInDate, CheckOutDate = dto.CheckOutDate, TotalPrice = dto.TotalPrice, Status = dto.Status, GuestId = dto.GuestId, RoomId = dto.RoomId };
        _repository.AddReservation(entity);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReservation), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateReservation(int id, ReservationUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = await _repository.GetReservationByIdAsync(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateReservation(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var entity = await _repository.GetReservationByIdAsync(id);
        if (entity == null) return NotFound();
        await _repository.DeleteReservationAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
