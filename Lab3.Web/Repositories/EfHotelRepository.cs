using Lab3.Model.Entities;
using Lab3.Model;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Web.Repositories;

public class EfHotelRepository : IHotelRepository
{
    private readonly HotelDbContext _context;

    public EfHotelRepository(HotelDbContext context)
    {
        _context = context;
    }

    public List<Hotel> GetAllHotels()
        => _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Employees)
            .Include(h => h.Services)
            .Include(h => h.Reservations)
            .ToList();

    public Hotel? GetHotelById(int id)
        => _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Employees)
            .Include(h => h.Services)
            .Include(h => h.Reservations)
            .SingleOrDefault(h => h.Id == id);

    public List<Room> GetAllRooms()
        => _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .ToList();

    public Room? GetRoomById(int id)
        => _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .SingleOrDefault(r => r.Id == id);

    public List<Service> GetAllServices()
        => _context.Services
            .Include(s => s.Hotel)
            .Include(s => s.Reservations)
            .ToList();

    public Service? GetServiceById(int id)
        => _context.Services
            .Include(s => s.Hotel)
            .Include(s => s.Reservations)
            .SingleOrDefault(s => s.Id == id);

    public List<Employee> GetAllEmployees()
        => _context.Employees
            .Include(e => e.Hotel)
            .ToList();

    public Employee? GetEmployeeById(int id)
        => _context.Employees
            .Include(e => e.Hotel)
            .SingleOrDefault(e => e.Id == id);

    public List<Guest> GetAllGuests()
        => _context.Guests
            .Include(g => g.Reservations)
            .Include(g => g.Reviews)
            .ToList();

    public Guest? GetGuestById(int id)
        => _context.Guests
            .Include(g => g.Reservations)
            .Include(g => g.Reviews)
            .SingleOrDefault(g => g.Id == id);

    public List<Reservation> GetAllReservations()
        => _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Include(r => r.Services)
            .Include(r => r.Payments)
            .ToList();

    public Reservation? GetReservationById(int id)
        => _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Include(r => r.Services)
            .Include(r => r.Payments)
            .SingleOrDefault(r => r.Id == id);

    public List<Payment> GetAllPayments()
        => _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Room)
                    .ThenInclude(room => room.Hotel)
            .ToList();

    public Payment? GetPaymentById(int id)
        => _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Room)
                    .ThenInclude(room => room.Hotel)
            .SingleOrDefault(p => p.Id == id);

    public List<Review> GetAllReviews()
        => _context.Reviews
            .Include(r => r.Guest)
            .Include(r => r.Hotel)
            .ToList();

    public Review? GetReviewById(int id)
        => _context.Reviews
            .Include(r => r.Guest)
            .Include(r => r.Hotel)
            .SingleOrDefault(r => r.Id == id);
}
