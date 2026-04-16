using Lab1.Model.Enums;

namespace Lab1.Model.Entities;

public class Reservation
{
    public int Id { get; set; }
    public string ReservationCode { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; }

    public Guest Guest { get; set; }
    public Room Room { get; set; }

    public List<Service> Services { get; set; }
    public List<Payment> Payments { get; set; }

    public Reservation()
    {
        Services = new List<Service>();
        Payments = new List<Payment>();
    }
}