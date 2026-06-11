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
    public ActionResult<IEnumerable<EmployeeDto>> GetEmployees([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? _repository.GetAllEmployees() : _repository.SearchEmployees(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public ActionResult<EmployeeDto> GetEmployee(int id)
    {
        var entity = _repository.GetEmployeeById(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<EmployeeDto> CreateEmployee(EmployeeCreateDto dto)
    {
        var entity = new Employee { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PhoneNumber = dto.PhoneNumber, Salary = dto.Salary, Role = dto.Role, HireDate = dto.HireDate, HotelId = dto.HotelId };
        _repository.AddEmployee(entity);
        _repository.SaveChanges();
        return CreatedAtAction(nameof(GetEmployee), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult UpdateEmployee(int id, EmployeeUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = _repository.GetEmployeeById(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdateEmployee(entity);
        _repository.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteEmployee(int id)
    {
        var entity = _repository.GetEmployeeById(id);
        if (entity == null) return NotFound();
        _repository.DeleteEmployee(id);
        _repository.SaveChanges();
        return NoContent();
    }
}
