using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

public class ServicesController : Controller
{
    private readonly IHotelRepository _repository;

    public ServicesController(IHotelRepository repository)
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
