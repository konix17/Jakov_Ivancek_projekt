using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HotelMgt.Web.Controllers;

[Route("placanja")]
public class PaymentsController : Controller
{
    private readonly IHotelRepository _repository;

    public PaymentsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var payments = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllPaymentsAsync() : await _repository.SearchPaymentsAsync(q);
        return View(payments);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var payments = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllPaymentsAsync() : await _repository.SearchPaymentsAsync(q);
        return PartialView("_PaymentsTable", payments);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var payments = await _repository.SearchPaymentsAsync(term);
        var results = payments.Select(p => new { id = p.Id, text = p.PaymentMethod.ToString(), meta = p.Reservation?.ReservationCode });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Reservations = await _repository.GetAllReservationsAsync();
        return View(new PaymentFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create(PaymentFormModel model)
    {
        model.PaymentDate = DateTime.Today;

        if (!ModelState.IsValid)
        {
            ViewBag.Reservations = await _repository.GetAllReservationsAsync();
            return View(model);
        }

        var payment = new Payment
        {
            Amount = model.Amount,
            PaymentDate = model.PaymentDate,
            PaymentMethod = model.PaymentMethod,
            IsPaid = model.IsPaid,
            ReservationId = model.ReservationId
        };

        _repository.AddPayment(payment);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var payment = await _repository.GetPaymentByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        ViewBag.Reservations = await _repository.GetAllReservationsAsync();
        return View(PaymentFormModel.FromEntity(payment));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, PaymentFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Reservations = await _repository.GetAllReservationsAsync();
            return View(model);
        }

        var payment = await _repository.GetPaymentByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        model.UpdateEntity(payment);
        _repository.UpdatePayment(payment);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var payment = await _repository.GetPaymentByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeletePaymentAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var payment = await _repository.GetPaymentByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }
}
