using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class GuestsController : Controller
{
    private readonly MockHotelRepository _repository;

    public GuestsController(MockHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllGuests());
    }

    public IActionResult Details(int id)
    {
        var guest = _repository.GetGuestById(id);
        if (guest == null)
        {
            return NotFound();
        }

        return View(guest);
    }
}
