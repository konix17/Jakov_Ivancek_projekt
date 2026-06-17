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
[Route("api/hotels")]
public class HotelsApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public HotelsApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels([FromQuery] string? q = null)
    {
        var hotels = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllHotelsAsync() : await _repository.SearchHotelsAsync(q);
        return hotels.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelDto>> GetHotel(int id)
    {
        var entity = await _repository.GetHotelByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HotelDto>> CreateHotel(HotelCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var entity = new Hotel
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            Rating = dto.Rating,
            PhoneNumber = dto.PhoneNumber
        };

        _repository.AddHotel(entity);
        await _repository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHotel), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateHotel(int id, HotelUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");

        var entity = await _repository.GetHotelByIdAsync(id);
        if (entity == null) return NotFound();

        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateHotel(entity);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var entity = await _repository.GetHotelByIdAsync(id);
        if (entity == null) return NotFound();

        await _repository.DeleteHotelAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
