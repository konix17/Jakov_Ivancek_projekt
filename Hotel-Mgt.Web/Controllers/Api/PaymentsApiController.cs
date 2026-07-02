using HotelMgt.Model.Entities;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/payments")]
public class PaymentsApiController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public PaymentsApiController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments([FromQuery] string? q = null)
    {
        var entities = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllPaymentsAsync() : await _repository.SearchPaymentsAsync(q);
        return entities.Select(ApiDtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(int id)
    {
        var entity = await _repository.GetPaymentByIdAsync(id);
        if (entity == null) return NotFound();
        return ApiDtoMapper.ToDto(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaymentDto>> CreatePayment(PaymentCreateDto dto)
    {
        var entity = new Payment { Amount = dto.Amount, PaymentDate = dto.PaymentDate, PaymentMethod = dto.PaymentMethod, IsPaid = dto.IsPaid, ReservationId = dto.ReservationId };
        _repository.AddPayment(entity);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPayment), new { id = entity.Id }, ApiDtoMapper.ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePayment(int id, PaymentUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Route id and payload id do not match.");
        var entity = await _repository.GetPaymentByIdAsync(id);
        if (entity == null) return NotFound();
        ApiDtoMapper.Apply(entity, dto);
        _repository.UpdatePayment(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        var entity = await _repository.GetPaymentByIdAsync(id);
        if (entity == null) return NotFound();
        await _repository.DeletePaymentAsync(id);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
