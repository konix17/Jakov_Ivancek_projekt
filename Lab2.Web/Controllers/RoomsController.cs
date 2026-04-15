using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class RoomsController : Controller
{
    private readonly MockHotelRepository _repository;

    public RoomsController(MockHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllRooms());
    }

    public IActionResult Details(int id)
    {
        var room = _repository.GetRoomById(id);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }
}
