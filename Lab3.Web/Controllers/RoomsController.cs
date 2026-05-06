using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

[Route("smjestaji")]
public class RoomsController : Controller
{
    private readonly IHotelRepository _repository;

    public RoomsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View(_repository.GetAllRooms());
    }

    [Route("{id:int}")]
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
