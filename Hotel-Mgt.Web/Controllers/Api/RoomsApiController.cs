using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/rooms")]
public class RoomsApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public RoomsApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<RoomDto>> GetRooms([FromQuery] string? q = null)
    {
        var rooms = string.IsNullOrWhiteSpace(q) ? _repository.GetAllRooms() : _repository.SearchRooms(q);
        return rooms.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public ActionResult<RoomDto> GetRoom(int id)
    {
        var entity = _repository.GetRoomById(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<RoomDto> CreateRoom(RoomCreateDto dto)
    {
        var entity = new Room
        {
            RoomNumber = dto.RoomNumber,
            Floor = dto.Floor,
            Capacity = dto.Capacity,
            PricePerNight = dto.PricePerNight,
            RoomType = dto.RoomType,
            IsAvailable = dto.IsAvailable,
            HotelId = dto.HotelId
        };

        _repository.AddRoom(entity);
        _repository.SaveChanges();

        return CreatedAtAction(nameof(GetRoom), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult UpdateRoom(int id, RoomUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = _repository.GetRoomById(id);
        if (entity == null) return NotFound();

        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateRoom(entity);
        _repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteRoom(int id)
    {
        var entity = _repository.GetRoomById(id);
        if (entity == null) return NotFound();

        _repository.DeleteRoom(id);
        _repository.SaveChanges();
        return NoContent();
    }
}
