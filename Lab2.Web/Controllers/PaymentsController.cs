using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class PaymentsController : Controller
{
    private readonly MockHotelRepository _repository;

    public PaymentsController(MockHotelRepository repository)
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
