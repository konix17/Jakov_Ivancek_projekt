using Microsoft.AspNetCore.Mvc;
using Lab3.Web.Repositories;

namespace Lab3.Web.Controllers;

[Route("hoteli")]
public class HotelsController : Controller
{
    private readonly IHotelRepository _repository;

    public HotelsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View(_repository.GetAllHotels());
    }

    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var hotel = _repository.GetHotelById(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return View(hotel);
    }
}
