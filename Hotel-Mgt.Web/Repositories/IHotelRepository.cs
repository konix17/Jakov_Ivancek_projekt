using HotelMgt.Model.Entities;

namespace HotelMgt.Web.Repositories;

public interface IHotelRepository
{
    List<Hotel> GetAllHotels();
    Hotel? GetHotelById(int id);
    List<Hotel> SearchHotels(string query);
    void AddHotel(Hotel hotel);
    void UpdateHotel(Hotel hotel);
    void DeleteHotel(int id);

    List<Room> GetAllRooms();
    Room? GetRoomById(int id);
    List<Room> SearchRooms(string query);
    List<Room> GetRoomsByHotel(int hotelId);
    void AddRoom(Room room);
    void UpdateRoom(Room room);
    void DeleteRoom(int id);

    List<Service> GetAllServices();
    Service? GetServiceById(int id);
    List<Service> SearchServices(string query);
    void AddService(Service service);
    void UpdateService(Service service);
    void DeleteService(int id);
    List<Service> GetServicesByIds(IEnumerable<int> ids);

    List<Employee> GetAllEmployees();
    Employee? GetEmployeeById(int id);
    List<Employee> SearchEmployees(string query);
    void AddEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void DeleteEmployee(int id);

    List<Guest> GetAllGuests();
    Guest? GetGuestById(int id);
    List<Guest> SearchGuests(string query);
    void AddGuest(Guest guest);
    void UpdateGuest(Guest guest);
    void DeleteGuest(int id);

    List<Reservation> GetAllReservations();
    Reservation? GetReservationById(int id);
    List<Reservation> SearchReservations(string query);
    void AddReservation(Reservation reservation);
    void UpdateReservation(Reservation reservation);
    void DeleteReservation(int id);

    List<Payment> GetAllPayments();
    Payment? GetPaymentById(int id);
    List<Payment> SearchPayments(string query);
    void AddPayment(Payment payment);
    void UpdatePayment(Payment payment);
    void DeletePayment(int id);

    List<Review> GetAllReviews();
    Review? GetReviewById(int id);
    List<Review> SearchReviews(string query);
    void AddReview(Review review);
    void UpdateReview(Review review);
    void DeleteReview(int id);

    bool SaveChanges();
}
