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
[Route("api/services")]
public class ServicesApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public ServicesApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllServicesAsync() : await _repository.SearchServicesAsync(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceDto>> GetService(int id)
    {
        var entity = await _repository.GetServiceByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ServiceDto>> CreateService(ServiceCreateDto dto)
    {
        var entity = new Service { Name = dto.Name, Description = dto.Description, Price = dto.Price, IsAvailable = dto.IsAvailable, HotelId = dto.HotelId };
        _repository.AddService(entity);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetService), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateService(int id, ServiceUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = await _repository.GetServiceByIdAsync(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateService(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var entity = await _repository.GetServiceByIdAsync(id);
        if (entity == null) return NotFound();
        await _repository.DeleteServiceAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
