using Microsoft.AspNetCore.Mvc;
using Lab2.Web.Repositories;

namespace Lab2.Web.Controllers;

public class HotelsController : Controller
{
    private readonly MockHotelRepository _repository;

    public HotelsController(MockHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View(_repository.GetAllHotels());
    }

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
