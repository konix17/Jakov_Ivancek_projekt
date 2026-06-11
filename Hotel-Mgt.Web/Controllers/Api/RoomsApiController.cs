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
[Route("api/rooms")]
public class RoomsApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public RoomsApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms([FromQuery] string? q = null)
    {
        var rooms = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllRoomsAsync() : await _repository.SearchRoomsAsync(q);
        return rooms.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomDto>> GetRoom(int id)
    {
        var entity = await _repository.GetRoomByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<RoomDto>> CreateRoom(RoomCreateDto dto)
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
        await _repository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateRoom(int id, RoomUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = await _repository.GetRoomByIdAsync(id);
        if (entity == null) return NotFound();

        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateRoom(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var entity = await _repository.GetRoomByIdAsync(id);
        if (entity == null) return NotFound();

        await _repository.DeleteRoomAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
