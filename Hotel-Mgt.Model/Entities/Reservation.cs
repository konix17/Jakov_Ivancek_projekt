using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelMgt.Model.Enums;

namespace HotelMgt.Model.Entities;

public class Reservation
{
    [Key]
    public int Id { get; set; }
    public string ReservationCode { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; }

    [ForeignKey("Guest")]
    public int GuestId { get; set; }
    public virtual Guest Guest { get; set; }

    [ForeignKey("Room")]
    public int RoomId { get; set; }
    public virtual Room Room { get; set; }

    public virtual ICollection<Service> Services { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }

    public Reservation()
    {
        Services = new List<Service>();
        Payments = new List<Payment>();
        ReservationCode = null!;
        Guest = null!;
        Room = null!;
    }
}