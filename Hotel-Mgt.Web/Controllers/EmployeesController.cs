using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var employees = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllEmployeesAsync() : await _repository.SearchEmployeesAsync(q);
        return View(employees);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var employees = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllEmployeesAsync() : await _repository.SearchEmployeesAsync(q);
        return PartialView("_EmployeesTable", employees);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var employees = await _repository.SearchEmployeesAsync(term);
        var results = employees.Select(e => new { id = e.Id, text = e.FirstName + " " + e.LastName, meta = e.Hotel?.Name });
        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(new EmployeeFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create(EmployeeFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
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
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        return View(EmployeeFormModel.FromEntity(employee));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, EmployeeFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hotels = await _repository.GetAllHotelsAsync();
            return View(model);
        }

        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        model.UpdateEntity(employee);
        _repository.UpdateEmployee(employee);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteEmployeeAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }
}
