using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HotelMgt.Model.Enums;
using HotelMgt.Web.Models;
using HotelMgt.Web.Repositories;

namespace HotelMgt.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHotelRepository _repository;

    public HomeController(IHotelRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        var hotels = _repository.GetAllHotels();
        var rooms = _repository.GetAllRooms();
        var guests = _repository.GetAllGuests();
        var reservations = _repository.GetAllReservations();
        var services = _repository.GetAllServices();
        var employees = _repository.GetAllEmployees();
        var payments = _repository.GetAllPayments();
        var reviews = _repository.GetAllReviews();

        var confirmedReservations = reservations.Where(r => r.Status == ReservationStatus.Confirmed).ToList();
        var occupiedRooms = rooms.Count(r => !r.IsAvailable);
        var totalRooms = rooms.Count;
        var availableRooms = rooms.Count(r => r.IsAvailable);
        var occupiedRate = totalRooms == 0 ? 0 : (int)Math.Round((double)occupiedRooms / totalRooms * 100);
        var availableServices = services.Count(s => s.IsAvailable);
        var serviceRate = services.Count == 0 ? 0 : (int)Math.Round((double)availableServices / services.Count * 100);
        var revenueCollected = payments.Where(p => p.IsPaid).Sum(p => p.Amount);
        var revenuePending = payments.Where(p => !p.IsPaid).Sum(p => p.Amount);
        var bestHotel = hotels
            .OrderByDescending(h => h.Rating)
            .ThenByDescending(h => h.Reservations.Count)
            .FirstOrDefault();
        var nextCheckIn = confirmedReservations
            .Where(r => r.CheckInDate >= DateTime.Today)
            .OrderBy(r => r.CheckInDate)
            .FirstOrDefault();
        var latestReview = reviews
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();
        var upcomingArrivals = confirmedReservations
            .Where(r => r.CheckInDate >= DateTime.Today)
            .OrderBy(r => r.CheckInDate)
            .Take(5)
            .Select(r => new UpcomingArrivalItem(
                $"{r.Guest.FirstName} {r.Guest.LastName}",
                r.Room.Hotel.Name,
                r.Room.RoomNumber,
                r.CheckInDate,
                r.Status.ToString()))
            .ToList();

        var model = new HomeViewModel
        {
            HotelsCount = hotels.Count,
            RoomsCount = rooms.Count,
            GuestsCount = guests.Count,
            ReservationsCount = reservations.Count,
            ServicesCount = services.Count,
            EmployeesCount = employees.Count,
            PaymentsCount = payments.Count,
            ReviewsCount = reviews.Count,
            ConfirmedReservationsCount = confirmedReservations.Count,
            OccupancyRate = occupiedRate,
            AvailableRoomsCount = availableRooms,
            OccupiedRoomsCount = occupiedRooms,
            ServiceAvailabilityRate = serviceRate,
            RevenueCollected = revenueCollected,
            RevenuePending = revenuePending,
            TopHotelName = bestHotel?.Name ?? "N/A",
            TopHotelRating = bestHotel?.Rating ?? 0,
            NextCheckIn = nextCheckIn is null
                ? "No confirmed arrivals"
                : $"{nextCheckIn.Guest.FirstName} {nextCheckIn.Guest.LastName} on {nextCheckIn.CheckInDate:MMM d}",
            LatestReviewSummary = latestReview is null
                ? "No reviews yet."
                : $"\"{latestReview.Comment}\" — {latestReview.Guest.FirstName} {latestReview.Guest.LastName}",
            UpcomingArrivals = upcomingArrivals
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
