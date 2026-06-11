using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<IEnumerable<ReservationDto>> GetReservations([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? _repository.GetAllReservations() : _repository.SearchReservations(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public ActionResult<ReservationDto> GetReservation(int id)
    {
        var entity = _repository.GetReservationById(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<ReservationDto> CreateReservation(ReservationCreateDto dto)
    {
        var entity = new Reservation { ReservationCode = dto.ReservationCode, ReservationDate = dto.ReservationDate, CheckInDate = dto.CheckInDate, CheckOutDate = dto.CheckOutDate, TotalPrice = dto.TotalPrice, Status = dto.Status, GuestId = dto.GuestId, RoomId = dto.RoomId };
        _repository.AddReservation(entity);
        _repository.SaveChanges();
        return CreatedAtAction(nameof(GetReservation), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult UpdateReservation(int id, ReservationUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = _repository.GetReservationById(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateReservation(entity);
        _repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteReservation(int id)
    {
        var entity = _repository.GetReservationById(id);
        if (entity == null) return NotFound();
        _repository.DeleteReservation(id);
        _repository.SaveChanges();
        return NoContent();
    }
}
