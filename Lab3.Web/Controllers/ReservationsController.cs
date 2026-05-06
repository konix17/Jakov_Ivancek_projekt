using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

[Route("rezervacije")]
public class ReservationsController : Controller
{
    private readonly IHotelRepository _repository;

    public ReservationsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View(_repository.GetAllReservations());
    }

    [Route("{id:int}")]
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
