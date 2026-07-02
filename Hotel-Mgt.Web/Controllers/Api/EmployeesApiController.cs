using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/employees")]
public class EmployeesApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public EmployeesApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllEmployeesAsync() : await _repository.SearchEmployeesAsync(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var entity = await _repository.GetEmployeeByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeCreateDto dto)
    {
        var entity = new Employee { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PhoneNumber = dto.PhoneNumber, Salary = dto.Salary, Role = dto.Role, HireDate = dto.HireDate, HotelId = dto.HotelId };
        _repository.AddEmployee(entity);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEmployee), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = await _repository.GetEmployeeByIdAsync(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateEmployee(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var entity = await _repository.GetEmployeeByIdAsync(id);
        if (entity == null) return NotFound();
        await _repository.DeleteEmployeeAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
