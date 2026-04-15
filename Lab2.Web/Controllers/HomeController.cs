using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Models;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class HomeController : Controller
{
    private readonly MockHotelRepository _repository;

    public HomeController(MockHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        var model = new HomeViewModel
        {
            HotelsCount = _repository.GetAllHotels().Count,
            RoomsCount = _repository.GetAllRooms().Count,
            GuestsCount = _repository.GetAllGuests().Count,
            ReservationsCount = _repository.GetAllReservations().Count,
            ServicesCount = _repository.GetAllServices().Count,
            EmployeesCount = _repository.GetAllEmployees().Count,
            PaymentsCount = _repository.GetAllPayments().Count,
            ReviewsCount = _repository.GetAllReviews().Count,
            ConfirmedReservationsCount = _repository.GetAllReservations().Count(r => r.Status == Lab1.Model.Enums.ReservationStatus.Confirmed)
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
