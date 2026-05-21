using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelMgt.Model.Enums;

namespace HotelMgt.Model.Entities;

public class Room
{
    [Key]
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public RoomType RoomType { get; set; }
    public bool IsAvailable { get; set; }

    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual Hotel Hotel { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; }

    public Room()
    {
        Reservations = new List<Reservation>();
    }
}