using Lab1.Model.Enums;

namespace Lab1.Model.Entities;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public RoomType RoomType { get; set; }
    public bool IsAvailable { get; set; }

    public Hotel Hotel { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Room()
    {
        Reservations = new List<Reservation>();
    }
}