using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

public class PaymentsController : Controller
{
    private readonly IHotelRepository _repository;

    public PaymentsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllPayments());
    }

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
