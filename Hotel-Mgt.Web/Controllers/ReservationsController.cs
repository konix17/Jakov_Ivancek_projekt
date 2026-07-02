using HotelMgt.Model.Entities;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Index(string q)
    {
        ViewData["SearchTerm"] = q;
        var reservations = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllReservationsAsync() : await _repository.SearchReservationsAsync(q);
        return View(reservations);
    }

    [Route("search")]
    public async Task<IActionResult> Search(string q)
    {
        var reservations = string.IsNullOrWhiteSpace(q) ? await _repository.GetAllReservationsAsync() : await _repository.SearchReservationsAsync(q);
        return PartialView("_ReservationsTable", reservations);
    }

    [Route("autocomplete")]
    public async Task<IActionResult> Autocomplete(string term)
    {
        var reservations = await _repository.SearchReservationsAsync(term);
        var results = reservations.Select(r => new { id = r.Id, text = r.ReservationCode, meta = r.Guest?.FirstName + " " + r.Guest?.LastName });
        return Json(results);
    }

    private async Task PopulateReservationFormDataAsync(ReservationFormModel model)
    {
        ViewBag.Hotels = await _repository.GetAllHotelsAsync();
        ViewBag.Guests = await _repository.GetAllGuestsAsync();
        ViewBag.Services = await _repository.GetAllServicesAsync();
        ViewBag.Rooms = model.HotelId > 0 ? await _repository.GetRoomsByHotelAsync(model.HotelId) : new List<Room>();
    }

    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        var model = new ReservationFormModel();
        await PopulateReservationFormDataAsync(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<IActionResult> Create(ReservationFormModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateReservationFormDataAsync(model);
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
            Services = await _repository.GetServicesByIdsAsync(model.SelectedServiceIds)
        };

        _repository.AddReservation(reservation);
        await _repository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var reservation = await _repository.GetReservationByIdAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        var model = ReservationFormModel.FromEntity(reservation);
        await PopulateReservationFormDataAsync(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, ReservationFormModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateReservationFormDataAsync(model);
            return View(model);
        }

        var reservation = await _repository.GetReservationByIdAsync(id);
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
        reservation.Services = await _repository.GetServicesByIdsAsync(model.SelectedServiceIds);

        _repository.UpdateReservation(reservation);
        await _repository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Route("hotel-rooms")]
    public async Task<IActionResult> HotelRooms(int hotelId)
    {
        if (hotelId <= 0)
        {
            return Json(new List<object>());
        }

        var rooms = await _repository.GetRoomsByHotelAsync(hotelId);
        var mapped = rooms.Select(r => new { id = r.Id, text = r.RoomNumber }).ToList();

        return Json(mapped);
    }

    [Authorize(Roles = "Admin")]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var reservation = await _repository.GetReservationByIdAsync(id);
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteReservationAsync(id);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var reservation = await _repository.GetReservationByIdAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        if (reservation.Room != null && reservation.Room.Hotel == null)
        {
            var hotel = await _repository.GetHotelByIdAsync(reservation.Room.HotelId);
            if (hotel != null)
            {
                reservation.Room.Hotel = hotel;
            }
        }

        return View(reservation);
    }
}
