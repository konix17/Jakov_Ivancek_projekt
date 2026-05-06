using Lab3.Model.Entities;

namespace Lab3.Web.Repositories;

public interface IHotelRepository
{
    List<Hotel> GetAllHotels();
    Hotel? GetHotelById(int id);

    List<Room> GetAllRooms();
    Room? GetRoomById(int id);

    List<Service> GetAllServices();
    Service? GetServiceById(int id);

    List<Employee> GetAllEmployees();
    Employee? GetEmployeeById(int id);

    List<Guest> GetAllGuests();
    Guest? GetGuestById(int id);

    List<Reservation> GetAllReservations();
    Reservation? GetReservationById(int id);

    List<Payment> GetAllPayments();
    Payment? GetPaymentById(int id);

    List<Review> GetAllReviews();
    Review? GetReviewById(int id);
}
