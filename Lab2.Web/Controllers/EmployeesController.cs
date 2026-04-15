using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly MockHotelRepository _repository;

    public EmployeesController(MockHotelRepository repository)
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
