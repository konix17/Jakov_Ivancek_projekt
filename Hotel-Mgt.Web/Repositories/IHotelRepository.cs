using HotelMgt.Model.Entities;

namespace HotelMgt.Web.Repositories;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllHotelsAsync();
    Task<Hotel?> GetHotelByIdAsync(int id);
    Task<List<Hotel>> SearchHotelsAsync(string query);
    void AddHotel(Hotel hotel);
    void UpdateHotel(Hotel hotel);
    Task DeleteHotelAsync(int id);

    Task<List<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByIdAsync(int id);
    Task<List<Room>> SearchRoomsAsync(string query);
    Task<List<Room>> GetRoomsByHotelAsync(int hotelId);
    void AddRoom(Room room);
    void UpdateRoom(Room room);
    Task DeleteRoomAsync(int id);

    Task<List<Service>> GetAllServicesAsync();
    Task<Service?> GetServiceByIdAsync(int id);
    Task<List<Service>> SearchServicesAsync(string query);
    void AddService(Service service);
    void UpdateService(Service service);
    Task DeleteServiceAsync(int id);
    Task<List<Service>> GetServicesByIdsAsync(IEnumerable<int> ids);

    Task<List<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<List<Employee>> SearchEmployeesAsync(string query);
    void AddEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    Task DeleteEmployeeAsync(int id);

    Task<List<Guest>> GetAllGuestsAsync();
    Task<Guest?> GetGuestByIdAsync(int id);
    Task<List<Guest>> SearchGuestsAsync(string query);
    void AddGuest(Guest guest);
    void UpdateGuest(Guest guest);
    Task DeleteGuestAsync(int id);

    Task<List<Reservation>> GetAllReservationsAsync();
    Task<Reservation?> GetReservationByIdAsync(int id);
    Task<List<Reservation>> SearchReservationsAsync(string query);
    void AddReservation(Reservation reservation);
    void UpdateReservation(Reservation reservation);
    Task DeleteReservationAsync(int id);

    Task<List<Payment>> GetAllPaymentsAsync();
    Task<Payment?> GetPaymentByIdAsync(int id);
    Task<List<Payment>> SearchPaymentsAsync(string query);
    void AddPayment(Payment payment);
    void UpdatePayment(Payment payment);
    Task DeletePaymentAsync(int id);

    Task<List<Review>> GetAllReviewsAsync();
    Task<Review?> GetReviewByIdAsync(int id);
    Task<List<Review>> SearchReviewsAsync(string query);
    void AddReview(Review review);
    void UpdateReview(Review review);
    Task DeleteReviewAsync(int id);

    Task<bool> SaveChangesAsync();
}
