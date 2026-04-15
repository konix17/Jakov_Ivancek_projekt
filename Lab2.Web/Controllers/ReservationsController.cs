using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class ReservationsController : Controller
{
    private readonly MockHotelRepository _repository;

    public ReservationsController(MockHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllReservations());
    }

    public IActionResult Details(int id)
    {
        var reservation = _repository.GetReservationById(id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }
}
