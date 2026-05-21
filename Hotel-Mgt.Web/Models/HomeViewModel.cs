namespace HotelMgt.Web.Models;

public class HomeViewModel
{
    public int HotelsCount { get; set; }
    public int RoomsCount { get; set; }
    public int GuestsCount { get; set; }
    public int ReservationsCount { get; set; }
    public int ServicesCount { get; set; }
    public int EmployeesCount { get; set; }
    public int PaymentsCount { get; set; }
    public int ReviewsCount { get; set; }
    public int ConfirmedReservationsCount { get; set; }
    public int OccupancyRate { get; set; }
    public int AvailableRoomsCount { get; set; }
    public int OccupiedRoomsCount { get; set; }
    public int ServiceAvailabilityRate { get; set; }
    public decimal RevenueCollected { get; set; }
    public decimal RevenuePending { get; set; }
    public string TopHotelName { get; set; } = string.Empty;
    public int TopHotelRating { get; set; }
    public string NextCheckIn { get; set; } = "No confirmed arrivals";
    public string LatestReviewSummary { get; set; } = "No reviews yet.";
    public List<UpcomingArrivalItem> UpcomingArrivals { get; set; } = new();
}

public record UpcomingArrivalItem(string GuestName, string HotelName, string RoomNumber, DateTime CheckInDate, string Status);
