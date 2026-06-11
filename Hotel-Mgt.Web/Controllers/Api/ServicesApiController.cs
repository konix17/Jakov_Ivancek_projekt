using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<IEnumerable<ServiceDto>> GetServices([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? _repository.GetAllServices() : _repository.SearchServices(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public ActionResult<ServiceDto> GetService(int id)
    {
        var entity = _repository.GetServiceById(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<ServiceDto> CreateService(ServiceCreateDto dto)
    {
        var entity = new Service { Name = dto.Name, Description = dto.Description, Price = dto.Price, IsAvailable = dto.IsAvailable, HotelId = dto.HotelId };
        _repository.AddService(entity);
        _repository.SaveChanges();
        return CreatedAtAction(nameof(GetService), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult UpdateService(int id, ServiceUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = _repository.GetServiceById(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateService(entity);
        _repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteService(int id)
    {
        var entity = _repository.GetServiceById(id);
        if (entity == null) return NotFound();
        _repository.DeleteService(id);
        _repository.SaveChanges();
        return NoContent();
    }
}
