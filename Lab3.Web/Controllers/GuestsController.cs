using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

[Route("gosti")]
public class GuestsController : Controller
{
    private readonly IHotelRepository _repository;

    public GuestsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View(_repository.GetAllGuests());
    }

    [Route("{id:int}")]
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
