using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var payments = string.IsNullOrWhiteSpace(q) ? _repository.GetAllPayments() : _repository.SearchPayments(q);
        return View(payments);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var payments = string.IsNullOrWhiteSpace(q) ? _repository.GetAllPayments() : _repository.SearchPayments(q);
        return PartialView("_PaymentsTable", payments);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchPayments(term)
            .Select(p => new { id = p.Id, text = p.PaymentMethod.ToString(), meta = p.Reservation?.ReservationCode });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create()
    {
        ViewBag.Reservations = _repository.GetAllReservations();
        return View(new PaymentFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create(PaymentFormModel model)
    {
        model.PaymentDate = DateTime.Today;

        if (!ModelState.IsValid)
        {
            ViewBag.Reservations = _repository.GetAllReservations();
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
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var payment = _repository.GetPaymentById(id);
        if (payment == null)
        {
            return NotFound();
        }

        ViewBag.Reservations = _repository.GetAllReservations();
        return View(PaymentFormModel.FromEntity(payment));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, PaymentFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Reservations = _repository.GetAllReservations();
            return View(model);
        }

        var payment = _repository.GetPaymentById(id);
        if (payment == null)
        {
            return NotFound();
        }

        model.UpdateEntity(payment);
        _repository.UpdatePayment(payment);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var payment = _repository.GetPaymentById(id);
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
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeletePayment(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var payment = _repository.GetPaymentById(id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }
}
