using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class ServicesController : Controller
{
    private readonly MockHotelRepository _repository;

    public ServicesController(MockHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllServices());
    }

    public IActionResult Details(int id)
    {
        var service = _repository.GetServiceById(id);
        if (service == null)
        {
            return NotFound();
        }

        return View(service);
    }
}
