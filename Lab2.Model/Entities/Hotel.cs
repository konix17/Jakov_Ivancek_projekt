using Lab1.Model.Entities;

namespace Lab1.Model.Entities;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public int Rating { get; set; }
    public string PhoneNumber { get; set; }

    public List<Room> Rooms { get; set; }
    public List<Employee> Employees { get; set; }
    public List<Service> Services { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Hotel()
    {
        Rooms = new List<Room>();
        Employees = new List<Employee>();
        Services = new List<Service>();
        Reservations = new List<Reservation>();
    }
}