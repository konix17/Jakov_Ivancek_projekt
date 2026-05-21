using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelMgt.Web.Controllers;

[Route("zaposlenici")]
public class EmployeesController : Controller
{
    private readonly IHotelRepository _repository;

    public EmployeesController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var employees = string.IsNullOrWhiteSpace(q) ? _repository.GetAllEmployees() : _repository.SearchEmployees(q);
        return View(employees);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var employees = string.IsNullOrWhiteSpace(q) ? _repository.GetAllEmployees() : _repository.SearchEmployees(q);
        return PartialView("_EmployeesTable", employees);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchEmployees(term)
            .Select(e => new { id = e.Id, text = e.FirstName + " " + e.LastName, meta = e.Hotel?.Name });
        return Json(results);
    }

    [Route("create")]
    public IActionResult Create()
    {
        ViewBag.Hotels = _repository.GetAllHotels();
        return View(new EmployeeFormModel());
    }

    [HttpPost]
    [Route("create")]
    public IActionResult Create(EmployeeFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = _repository.GetAllHotels();
            return View(model);
        }

        var employee = new Employee
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Salary = model.Salary,
            Role = model.Role,
            HireDate = model.HireDate,
            HotelId = model.HotelId
        };

        _repository.AddEmployee(employee);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var employee = _repository.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }

        ViewBag.Hotels = _repository.GetAllHotels();
        return View(EmployeeFormModel.FromEntity(employee));
    }

    [HttpPost]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, EmployeeFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = _repository.GetAllHotels();
            return View(model);
        }

        var employee = _repository.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }

        model.UpdateEntity(employee);
        _repository.UpdateEmployee(employee);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var employee = _repository.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpPost]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteEmployee(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [Route("{id:int}")]
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
