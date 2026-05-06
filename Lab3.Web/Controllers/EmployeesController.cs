using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly IHotelRepository _repository;

    public EmployeesController(IHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllEmployees());
    }

    public IActionResult Details(int id)
    {
        var employee = _repository.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }
}
