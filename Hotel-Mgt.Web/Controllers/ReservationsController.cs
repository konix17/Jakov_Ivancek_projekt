using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelMgt.Web.Controllers;

[Route("rezervacije")]
public class ReservationsController : Controller
{
    private readonly IHotelRepository _repository;

    public ReservationsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    public IActionResult Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var reservations = string.IsNullOrWhiteSpace(q) ? _repository.GetAllReservations() : _repository.SearchReservations(q);
        return View(reservations);
    }

    [Route("search")]
    public IActionResult Search(string q)
    {
        var reservations = string.IsNullOrWhiteSpace(q) ? _repository.GetAllReservations() : _repository.SearchReservations(q);
        return PartialView("_ReservationsTable", reservations);
    }

    [Route("autocomplete")]
    public IActionResult Autocomplete(string term)
    {
        var results = _repository.SearchReservations(term)
            .Select(r => new { id = r.Id, text = r.ReservationCode, meta = r.Guest?.FirstName + " " + r.Guest?.LastName });
        return Json(results);
    }

    private void PopulateReservationFormData(ReservationFormModel model)
    {
        ViewBag.Hotels = _repository.GetAllHotels();
        ViewBag.Guests = _repository.GetAllGuests();
        ViewBag.Services = _repository.GetAllServices();
        ViewBag.Rooms = model.HotelId > 0 ? _repository.GetRoomsByHotel(model.HotelId) : new List<Room>();
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create()
    {
        var model = new ReservationFormModel();
        PopulateReservationFormData(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public IActionResult Create(ReservationFormModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateReservationFormData(model);
            return View(model);
        }

        var reservation = new Reservation
        {
            ReservationCode = model.ReservationCode,
            ReservationDate = model.ReservationDate,
            CheckInDate = model.CheckInDate,
            CheckOutDate = model.CheckOutDate,
            TotalPrice = model.TotalPrice,
            Status = model.Status,
            GuestId = model.GuestId,
            RoomId = model.RoomId,
            Services = _repository.GetServicesByIds(model.SelectedServiceIds)
        };

        _repository.AddReservation(reservation);
        _repository.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var reservation = _repository.GetReservationById(id);
        if (reservation == null)
        {
            return NotFound();
        }

        var model = ReservationFormModel.FromEntity(reservation);
        PopulateReservationFormData(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public IActionResult Edit(int id, ReservationFormModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateReservationFormData(model);
            return View(model);
        }

        var reservation = _repository.GetReservationById(id);
        if (reservation == null)
        {
            return NotFound();
        }

        reservation.ReservationCode = model.ReservationCode;
        reservation.ReservationDate = model.ReservationDate;
        reservation.CheckInDate = model.CheckInDate;
        reservation.CheckOutDate = model.CheckOutDate;
        reservation.TotalPrice = model.TotalPrice;
        reservation.Status = model.Status;
        reservation.GuestId = model.GuestId;
        reservation.RoomId = model.RoomId;
        reservation.Services = _repository.GetServicesByIds(model.SelectedServiceIds);

        _repository.UpdateReservation(reservation);
        _repository.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Route("hotel-rooms")]
    public IActionResult HotelRooms(int hotelId)
    {
        if (hotelId <= 0)
        {
            return Json(new List<object>());
        }

        var rooms = _repository.GetRoomsByHotel(hotelId)
            .Select(r => new { id = r.Id, text = r.RoomNumber })
            .ToList();

        return Json(rooms);
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var reservation = _repository.GetReservationById(id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("Delete")]
    [Route("delete/{id:int}")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repository.DeleteReservation(id);
        _repository.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var reservation = _repository.GetReservationById(id);
        if (reservation == null)
        {
            return NotFound();
        }

        if (reservation.Room != null && reservation.Room.Hotel == null)
        {
            var hotel = _repository.GetHotelById(reservation.Room.HotelId);
            if (hotel != null)
            {
                reservation.Room.Hotel = hotel;
            }
        }

        return View(reservation);
    }
}
