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

    public async Task<List<Hotel>> GetAllHotelsAsync()
        => await _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Employees)
            .Include(h => h.Services)
            .Include(h => h.Reservations)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Hotel?> GetHotelByIdAsync(int id)
        => await _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Employees)
            .Include(h => h.Services)
            .Include(h => h.Reservations)
            .SingleOrDefaultAsync(h => h.Id == id);

    public async Task<List<Hotel>> SearchHotelsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllHotelsAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Employees)
            .Include(h => h.Services)
            .Include(h => h.Reservations)
            .Where(h => EF.Functions.Like(h.Name, wildcard)
                     || EF.Functions.Like(h.City, wildcard)
                     || EF.Functions.Like(h.Address, wildcard)
                     || EF.Functions.Like(h.PhoneNumber, wildcard))
            .AsNoTracking()
            .ToListAsync();
    }

    public void AddHotel(Hotel hotel) => _context.Hotels.Add(hotel);
    public void UpdateHotel(Hotel hotel) => _context.Hotels.Update(hotel);
    public async Task DeleteHotelAsync(int id)
    {
        var exists = await _context.Hotels.AnyAsync(h => h.Id == id);
        if (!exists)
        {
            return;
        }

        _context.ChangeTracker.Clear();

        // The Service -> Hotel foreign key does not cascade at the database level,
        // so dependent services must be removed explicitly before the hotel itself.
        var services = await _context.Services.Where(s => s.HotelId == id).ToListAsync();
        _context.Services.RemoveRange(services);

        _context.Hotels.Remove(new Hotel { Id = id });
    }

    public async Task<List<Room>> GetAllRoomsAsync()
        => await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Room?> GetRoomByIdAsync(int id)
        => await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .SingleOrDefaultAsync(r => r.Id == id);

    public async Task<List<Room>> GetRoomsByHotelAsync(int hotelId)
        => await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .Where(r => r.HotelId == hotelId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Room>> SearchRoomsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllRoomsAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservations)
            .Where(r => EF.Functions.Like(r.RoomNumber, wildcard)
                     || EF.Functions.Like(r.Hotel.Name, wildcard)
                     || EF.Functions.Like(r.Hotel.City, wildcard))
            .AsNoTracking()
            .ToListAsync();
    }

    public void AddRoom(Room room) => _context.Rooms.Add(room);
    public void UpdateRoom(Room room) => _context.Rooms.Update(room);
    public async Task DeleteRoomAsync(int id)
    {
        var entity = await _context.Rooms.FindAsync(id);
        if (entity != null) _context.Rooms.Remove(entity);
    }

    public async Task<List<Service>> GetAllServicesAsync()
        => await _context.Services
            .Include(s => s.Hotel)
            .Include(s => s.Reservations)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Service?> GetServiceByIdAsync(int id)
        => await _context.Services
            .Include(s => s.Hotel)
            .Include(s => s.Reservations)
            .SingleOrDefaultAsync(s => s.Id == id);

    public async Task<List<Service>> SearchServicesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllServicesAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Services
            .Include(s => s.Hotel)
            .Include(s => s.Reservations)
            .Where(s => EF.Functions.Like(s.Name, wildcard)
                     || EF.Functions.Like(s.Description, wildcard)
                     || EF.Functions.Like(s.Hotel.Name, wildcard))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Service>> GetServicesByIdsAsync(IEnumerable<int> ids)
        => await _context.Services
            .Where(s => ids.Contains(s.Id))
            .ToListAsync();

    public void AddService(Service service) => _context.Services.Add(service);
    public void UpdateService(Service service) => _context.Services.Update(service);
    public async Task DeleteServiceAsync(int id)
    {
        var entity = await _context.Services.FindAsync(id);
        if (entity != null) _context.Services.Remove(entity);
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
        => await _context.Employees
            .Include(e => e.Hotel)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
        => await _context.Employees
            .Include(e => e.Hotel)
            .SingleOrDefaultAsync(e => e.Id == id);

    public async Task<List<Employee>> SearchEmployeesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllEmployeesAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Employees
            .Include(e => e.Hotel)
            .Where(e => EF.Functions.Like(e.FirstName, wildcard)
                     || EF.Functions.Like(e.LastName, wildcard)
                     || EF.Functions.Like(e.Email, wildcard)
                     || EF.Functions.Like(e.PhoneNumber, wildcard)
                     || EF.Functions.Like(e.Hotel.Name, wildcard))
            .AsNoTracking()
            .ToListAsync();
    }

    public void AddEmployee(Employee employee) => _context.Employees.Add(employee);
    public void UpdateEmployee(Employee employee) => _context.Employees.Update(employee);
    public async Task DeleteEmployeeAsync(int id)
    {
        var entity = await _context.Employees.FindAsync(id);
        if (entity != null) _context.Employees.Remove(entity);
    }

    public async Task<List<Guest>> GetAllGuestsAsync()
        => await _context.Guests
            .Include(g => g.Reservations)
            .Include(g => g.Reviews)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Guest?> GetGuestByIdAsync(int id)
        => await _context.Guests
            .Include(g => g.Reservations)
            .Include(g => g.Reviews)
            .SingleOrDefaultAsync(g => g.Id == id);

    public async Task<List<Guest>> SearchGuestsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllGuestsAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Guests
            .Include(g => g.Reservations)
            .Include(g => g.Reviews)
            .Where(g => EF.Functions.Like(g.FirstName, wildcard)
                     || EF.Functions.Like(g.LastName, wildcard)
                     || EF.Functions.Like(g.Email, wildcard)
                     || EF.Functions.Like(g.PhoneNumber, wildcard)
                     || EF.Functions.Like(g.DocumentNumber, wildcard))
            .AsNoTracking()
            .ToListAsync();
    }

    public void AddGuest(Guest guest) => _context.Guests.Add(guest);
    public void UpdateGuest(Guest guest) => _context.Guests.Update(guest);
    public async Task DeleteGuestAsync(int id)
    {
        var entity = await _context.Guests.FindAsync(id);
        if (entity != null) _context.Guests.Remove(entity);
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
        => await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Include(r => r.Services)
            .Include(r => r.Payments)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Reservation?> GetReservationByIdAsync(int id)
        => await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Include(r => r.Services)
            .Include(r => r.Payments)
            .SingleOrDefaultAsync(r => r.Id == id);

    public async Task<List<Reservation>> SearchReservationsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllReservationsAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Reservations
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
            .AsNoTracking()
            .ToListAsync();
    }

    public void AddReservation(Reservation reservation) => _context.Reservations.Add(reservation);
    public void UpdateReservation(Reservation reservation) => _context.Reservations.Update(reservation);
    public async Task DeleteReservationAsync(int id)
    {
        var entity = await _context.Reservations.FindAsync(id);
        if (entity != null) _context.Reservations.Remove(entity);
    }

    public async Task<List<Payment>> GetAllPaymentsAsync()
        => await _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Room)
                    .ThenInclude(room => room.Hotel)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Payment?> GetPaymentByIdAsync(int id)
        => await _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Room)
                    .ThenInclude(room => room.Hotel)
            .SingleOrDefaultAsync(p => p.Id == id);

    public async Task<List<Payment>> SearchPaymentsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllPaymentsAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Payments
            .Include(p => p.Reservation)
                .ThenInclude(r => r.Room)
                    .ThenInclude(room => room.Hotel)
            .Where(p => EF.Functions.Like(p.PaymentMethod.ToString(), wildcard)
                     || EF.Functions.Like(p.Reservation.ReservationCode, wildcard)
                     || EF.Functions.Like(p.Reservation.Room.RoomNumber, wildcard))
            .AsNoTracking()
            .ToListAsync();
    }

    public void AddPayment(Payment payment) => _context.Payments.Add(payment);
    public void UpdatePayment(Payment payment) => _context.Payments.Update(payment);
    public async Task DeletePaymentAsync(int id)
    {
        var entity = await _context.Payments.FindAsync(id);
        if (entity != null) _context.Payments.Remove(entity);
    }

    public async Task<List<Review>> GetAllReviewsAsync()
        => await _context.Reviews
            .Include(r => r.Guest)
            .Include(r => r.Hotel)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Review?> GetReviewByIdAsync(int id)
        => await _context.Reviews
            .Include(r => r.Guest)
            .Include(r => r.Hotel)
            .SingleOrDefaultAsync(r => r.Id == id);

    public async Task<List<Review>> SearchReviewsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllReviewsAsync();
        }

        var wildcard = $"%{query}%";
        return await _context.Reviews
            .Include(r => r.Guest)
            .Include(r => r.Hotel)
            .Where(r => EF.Functions.Like(r.Comment, wildcard)
                     || EF.Functions.Like(r.Guest.FirstName, wildcard)
                     || EF.Functions.Like(r.Guest.LastName, wildcard)
                     || EF.Functions.Like(r.Hotel.Name, wildcard)
                     || EF.Functions.Like(r.Rating.ToString(), wildcard))
            .AsNoTracking()
            .ToListAsync();
    }

    public void AddReview(Review review) => _context.Reviews.Add(review);
    public void UpdateReview(Review review) => _context.Reviews.Update(review);
    public async Task DeleteReviewAsync(int id)
    {
        var entity = await _context.Reviews.FindAsync(id);
        if (entity != null) _context.Reviews.Remove(entity);
    }

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() >= 0;
}
