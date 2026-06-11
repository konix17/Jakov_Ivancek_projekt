using HotelMgt.Model;
using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/hotels")]
public class HotelsApiController : ControllerBase
{
    private readonly HotelDbContext _context;

    public HotelsApiController(HotelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels([FromQuery] string? q = null)
    {
        var query = _context.Hotels.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(h => h.Name.Contains(q) || h.City.Contains(q));
        }
        var hotels = await query.ToListAsync();
        return hotels.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelDto>> GetHotel(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return NotFound();
        return ApiDtoMapper.ToDto(hotel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
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

        _context.Hotels.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHotel), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateHotel(int id, HotelUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");

        var entity = await _context.Hotels.FindAsync(id);
        if (entity == null) return NotFound();

        ApiDtoMapper.Apply(entity, dto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var entity = await _context.Hotels.FindAsync(id);
        if (entity == null) return NotFound();

        _context.Hotels.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
