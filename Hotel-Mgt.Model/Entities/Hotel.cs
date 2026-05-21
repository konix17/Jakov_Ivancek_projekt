using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelMgt.Model.Entities;

public class Hotel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public int Rating { get; set; }
    public string PhoneNumber { get; set; }

    public virtual ICollection<Room> Rooms { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
    public virtual ICollection<Service> Services { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }

    public Hotel()
    {
        Rooms = new List<Room>();
        Employees = new List<Employee>();
        Services = new List<Service>();
        Reservations = new List<Reservation>();
        Reviews = new List<Review>();
    }
}