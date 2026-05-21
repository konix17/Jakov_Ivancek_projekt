using HotelMgt.Model.Entities;
using HotelMgt.Model;
using Microsoft.EntityFrameworkCore;

namespace HotelMgt.Web.Repositories;

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

    public List<Hotel> SearchHotels(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllHotels();
        }

        var wildcard = $"%{query}%";
        return _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Employees)
            .Include(h => h.Services)
            .Include(h => h.Reservations)
            .Where(h => EF.Functions.Like(h.Name, wildcard)
                     || EF.Functions.Like(h.City, wildcard)
                     || EF.Functions.Like(h.Address, wildcard)
                     || EF.Functions.Like(h.PhoneNumber, wildcard))
            .ToList();
    }

    public void AddHotel(Hotel hotel) => _context.Hotels.Add(hotel);
    public void UpdateHotel(Hotel hotel) => _context.Hotels.Update(hotel);
    public void DeleteHotel(int id)
    {
        var hotel = GetHotelById(id);
        if (hotel != null)
        {
            _context.Hotels.Remove(hotel);
        }
    }

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

    public List<Room> GetRoomsByHotel(int hotelId)
        => _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .Where(r => r.HotelId == hotelId)
            .ToList();

    public List<Room> SearchRooms(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllRooms();
        }

        var wildcard = $"%{query}%";
        return _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .Where(r => EF.Functions.Like(r.RoomNumber, wildcard)
                     || EF.Functions.Like(r.Hotel.Name, wildcard)
                     || EF.Functions.Like(r.Hotel.City, wildcard))
            .ToList();
    }

    public void AddRoom(Room room) => _context.Rooms.Add(room);
    public void UpdateRoom(Room room) => _context.Rooms.Update(room);
    public void DeleteRoom(int id)
    {
        var room = GetRoomById(id);
        if (room != null)
        {
            _context.Rooms.Remove(room);
        }
    }

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

    public List<Service> SearchServices(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllServices();
        }

        var wildcard = $"%{query}%";
        return _context.Services
            .Include(s => s.Hotel)
            .Include(s => s.Reservations)
            .Where(s => EF.Functions.Like(s.Name, wildcard)
                     || EF.Functions.Like(s.Description, wildcard)
                     || EF.Functions.Like(s.Hotel.Name, wildcard))
            .ToList();
    }

    public List<Service> GetServicesByIds(IEnumerable<int> ids)
        => _context.Services
            .Where(s => ids.Contains(s.Id))
            .ToList();

    public void AddService(Service service) => _context.Services.Add(service);
    public void UpdateService(Service service) => _context.Services.Update(service);
    public void DeleteService(int id)
    {
        var service = GetServiceById(id);
        if (service != null)
        {
            _context.Services.Remove(service);
        }
    }

    public List<Employee> GetAllEmployees()
        => _context.Employees
            .Include(e => e.Hotel)
            .ToList();

    public Employee? GetEmployeeById(int id)
        => _context.Employees
            .Include(e => e.Hotel)
            .SingleOrDefault(e => e.Id == id);

    public List<Employee> SearchEmployees(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllEmployees();
        }

        var wildcard = $"%{query}%";
        return _context.Employees
            .Include(e => e.Hotel)
            .Where(e => EF.Functions.Like(e.FirstName, wildcard)
                     || EF.Functions.Like(e.LastName, wildcard)
                     || EF.Functions.Like(e.Email, wildcard)
                     || EF.Functions.Like(e.PhoneNumber, wildcard)
                     || EF.Functions.Like(e.Hotel.Name, wildcard))
            .ToList();
    }

    public void AddEmployee(Employee employee) => _context.Employees.Add(employee);
    public void UpdateEmployee(Employee employee) => _context.Employees.Update(employee);
    public void DeleteEmployee(int id)
    {
        var employee = GetEmployeeById(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
        }
    }

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

    public List<Guest> SearchGuests(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllGuests();
        }

        var wildcard = $"%{query}%";
        return _context.Guests
            .Include(g => g.Reservations)
            .Include(g => g.Reviews)
            .Where(g => EF.Functions.Like(g.FirstName, wildcard)
                     || EF.Functions.Like(g.LastName, wildcard)
                     || EF.Functions.Like(g.Email, wildcard)
                     || EF.Functions.Like(g.PhoneNumber, wildcard)
                     || EF.Functions.Like(g.DocumentNumber, wildcard))
            .ToList();
    }

    public void AddGuest(Guest guest) => _context.Guests.Add(guest);
    public void UpdateGuest(Guest guest) => _context.Guests.Update(guest);
    public void DeleteGuest(int id)
    {
        var guest = GetGuestById(id);
        if (guest != null)
        {
            _context.Guests.Remove(guest);
        }
    }

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

    public List<Reservation> SearchReservations(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllReservations();
        }

        var wildcard = $"%{query}%";
        return _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Include(r => r.Services)
            .Include(r => r.Payments)
            .Where(r => EF.Functions.Like(r.ReservationCode, wildcard)
                     || EF.Functions.Like(r.Status.ToString(), wildcard)
                     || EF.Functions.Like(r.Guest.FirstName, wildcard)
                     || EF.Functions.Like(r.Guest.LastName, wildcard)
                     || EF.Functions.Like(r.Room.RoomNumber, wildcard)
                     || EF.Functions.Like(r.Room.Hotel.Name, wildcard))
            .ToList();
    }

    public void AddReservation(Reservation reservation) => _context.Reservations.Add(reservation);
    public void UpdateReservation(Reservation reservation) => _context.Reservations.Update(reservation);
    public void DeleteReservation(int id)
    {
        var reservation = GetReservationById(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
        }
    }

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

    public List<Payment> SearchPayments(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllPayments();
        }

        var wildcard = $"%{query}%";
        return _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Room)
                    .ThenInclude(room => room.Hotel)
            .Where(p => EF.Functions.Like(p.PaymentMethod.ToString(), wildcard)
                     || EF.Functions.Like(p.Reservation.ReservationCode, wildcard)
                     || EF.Functions.Like(p.Reservation.Room.RoomNumber, wildcard))
            .ToList();
    }

    public void AddPayment(Payment payment) => _context.Payments.Add(payment);
    public void UpdatePayment(Payment payment) => _context.Payments.Update(payment);
    public void DeletePayment(int id)
    {
        var payment = GetPaymentById(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
        }
    }

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

    public List<Review> SearchReviews(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllReviews();
        }

        var wildcard = $"%{query}%";
        return _context.Reviews
            .Include(r => r.Guest)
            .Include(r => r.Hotel)
            .Where(r => EF.Functions.Like(r.Comment, wildcard)
                     || EF.Functions.Like(r.Guest.FirstName, wildcard)
                     || EF.Functions.Like(r.Guest.LastName, wildcard)
                     || EF.Functions.Like(r.Hotel.Name, wildcard)
                     || EF.Functions.Like(r.Rating.ToString(), wildcard))
            .ToList();
    }

    public void AddReview(Review review) => _context.Reviews.Add(review);
    public void UpdateReview(Review review) => _context.Reviews.Update(review);
    public void DeleteReview(int id)
    {
        var review = GetReviewById(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
        }
    }

    public bool SaveChanges() => _context.SaveChanges() >= 0;
}
