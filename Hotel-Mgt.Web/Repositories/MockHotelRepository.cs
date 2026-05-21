using HotelMgt.Model.Entities;
using HotelMgt.Model.Enums;

namespace HotelMgt.Web.Repositories;

public class MockHotelRepository
{
    public List<Hotel> Hotels { get; }
    public List<Room> Rooms { get; }
    public List<Service> Services { get; }
    public List<Employee> Employees { get; }
    public List<Guest> Guests { get; }
    public List<Reservation> Reservations { get; }
    public List<Payment> Payments { get; }
    public List<Review> Reviews { get; }

    public MockHotelRepository()
    {
        var hotel1 = new Hotel
        {
            Id = 1,
            Name = "Adriatic Hotel",
            Address = "Ilica 10",
            City = "Zagreb",
            Rating = 4,
            PhoneNumber = "0911111111"
        };

        var hotel2 = new Hotel
        {
            Id = 2,
            Name = "Sunset Resort",
            Address = "Riva 20",
            City = "Split",
            Rating = 5,
            PhoneNumber = "0922222222"
        };

        var hotel3 = new Hotel
        {
            Id = 3,
            Name = "Mountain View Hotel",
            Address = "Korzo 5",
            City = "Rijeka",
            Rating = 3,
            PhoneNumber = "0933333333"
        };

        var room1 = new Room
        {
            Id = 1,
            RoomNumber = "101",
            Floor = 1,
            Capacity = 1,
            PricePerNight = 80,
            RoomType = RoomType.Single,
            IsAvailable = true,
            Hotel = hotel1
        };

        var room2 = new Room
        {
            Id = 2,
            RoomNumber = "102",
            Floor = 1,
            Capacity = 2,
            PricePerNight = 120,
            RoomType = RoomType.Double,
            IsAvailable = true,
            Hotel = hotel1
        };

        var room3 = new Room
        {
            Id = 3,
            RoomNumber = "201",
            Floor = 2,
            Capacity = 3,
            PricePerNight = 250,
            RoomType = RoomType.Suite,
            IsAvailable = false,
            Hotel = hotel2
        };

        var room4 = new Room
        {
            Id = 4,
            RoomNumber = "202",
            Floor = 2,
            Capacity = 2,
            PricePerNight = 150,
            RoomType = RoomType.Double,
            IsAvailable = true,
            Hotel = hotel2
        };

        var room5 = new Room
        {
            Id = 5,
            RoomNumber = "301",
            Floor = 3,
            Capacity = 1,
            PricePerNight = 70,
            RoomType = RoomType.Single,
            IsAvailable = true,
            Hotel = hotel3
        };

        var room6 = new Room
        {
            Id = 6,
            RoomNumber = "302",
            Floor = 3,
            Capacity = 3,
            PricePerNight = 180,
            RoomType = RoomType.Suite,
            IsAvailable = false,
            Hotel = hotel3
        };

        hotel1.Rooms.Add(room1);
        hotel1.Rooms.Add(room2);
        hotel2.Rooms.Add(room3);
        hotel2.Rooms.Add(room4);
        hotel3.Rooms.Add(room5);
        hotel3.Rooms.Add(room6);

        var service1 = new Service
        {
            Id = 1,
            Name = "Breakfast",
            Description = "Buffet breakfast",
            Price = 15,
            IsAvailable = true,
            Hotel = hotel1
        };

        var service2 = new Service
        {
            Id = 2,
            Name = "Parking",
            Description = "Private hotel parking",
            Price = 10,
            IsAvailable = true,
            Hotel = hotel1
        };

        var service3 = new Service
        {
            Id = 3,
            Name = "Spa",
            Description = "Spa and wellness access",
            Price = 30,
            IsAvailable = true,
            Hotel = hotel2
        };

        var service4 = new Service
        {
            Id = 4,
            Name = "Dinner",
            Description = "Hotel restaurant dinner",
            Price = 25,
            IsAvailable = true,
            Hotel = hotel3
        };

        hotel1.Services.Add(service1);
        hotel1.Services.Add(service2);
        hotel2.Services.Add(service3);
        hotel3.Services.Add(service4);

        var employee1 = new Employee
        {
            Id = 1,
            FirstName = "Marko",
            LastName = "Maric",
            Email = "marko@adriatic.hr",
            PhoneNumber = "0991111111",
            Salary = 1200,
            Role = EmployeeRole.Manager,
            HireDate = new DateTime(2022, 5, 10),
            Hotel = hotel1
        };

        var employee2 = new Employee
        {
            Id = 2,
            FirstName = "Ivana",
            LastName = "Ivic",
            Email = "ivana@sunset.hr",
            PhoneNumber = "0992222222",
            Salary = 1000,
            Role = EmployeeRole.Receptionist,
            HireDate = new DateTime(2023, 2, 15),
            Hotel = hotel2
        };

        var employee3 = new Employee
        {
            Id = 3,
            FirstName = "Petra",
            LastName = "Peric",
            Email = "petra@mountain.hr",
            PhoneNumber = "0993333333",
            Salary = 900,
            Role = EmployeeRole.Housekeeping,
            HireDate = new DateTime(2024, 1, 20),
            Hotel = hotel3
        };

        hotel1.Employees.Add(employee1);
        hotel2.Employees.Add(employee2);
        hotel3.Employees.Add(employee3);

        var guest1 = new Guest
        {
            Id = 1,
            FirstName = "Ivan",
            LastName = "Ivic",
            Email = "ivan@gmail.com",
            PhoneNumber = "0951111111",
            DateOfBirth = new DateTime(1999, 5, 10),
            DocumentNumber = "HR12345"
        };

        var guest2 = new Guest
        {
            Id = 2,
            FirstName = "Ana",
            LastName = "Anic",
            Email = "ana@gmail.com",
            PhoneNumber = "0952222222",
            DateOfBirth = new DateTime(2000, 8, 15),
            DocumentNumber = "HR67890"
        };

        var guest3 = new Guest
        {
            Id = 3,
            FirstName = "Luka",
            LastName = "Lukic",
            Email = "luka@gmail.com",
            PhoneNumber = "0953333333",
            DateOfBirth = new DateTime(1998, 3, 22),
            DocumentNumber = "HR54321"
        };

        var reservation1 = new Reservation
        {
            Id = 1,
            ReservationCode = "RES001",
            ReservationDate = new DateTime(2026, 3, 20),
            CheckInDate = new DateTime(2026, 4, 10),
            CheckOutDate = new DateTime(2026, 4, 15),
            TotalPrice = 400,
            Status = ReservationStatus.Confirmed,
            Guest = guest1,
            Room = room1
        };

        var reservation2 = new Reservation
        {
            Id = 2,
            ReservationCode = "RES002",
            ReservationDate = new DateTime(2026, 3, 22),
            CheckInDate = new DateTime(2026, 5, 1),
            CheckOutDate = new DateTime(2026, 5, 3),
            TotalPrice = 240,
            Status = ReservationStatus.Pending,
            Guest = guest2,
            Room = room2
        };

        var reservation3 = new Reservation
        {
            Id = 3,
            ReservationCode = "RES003",
            ReservationDate = new DateTime(2026, 3, 25),
            CheckInDate = new DateTime(2026, 6, 5),
            CheckOutDate = new DateTime(2026, 6, 10),
            TotalPrice = 900,
            Status = ReservationStatus.Confirmed,
            Guest = guest3,
            Room = room3
        };

        var reservation4 = new Reservation
        {
            Id = 4,
            ReservationCode = "RES004",
            ReservationDate = new DateTime(2026, 3, 27),
            CheckInDate = new DateTime(2026, 7, 1),
            CheckOutDate = new DateTime(2026, 7, 4),
            TotalPrice = 540,
            Status = ReservationStatus.Cancelled,
            Guest = guest1,
            Room = room6
        };

        guest1.Reservations.Add(reservation1);
        guest1.Reservations.Add(reservation4);
        guest2.Reservations.Add(reservation2);
        guest3.Reservations.Add(reservation3);

        room1.Reservations.Add(reservation1);
        room2.Reservations.Add(reservation2);
        room3.Reservations.Add(reservation3);
        room6.Reservations.Add(reservation4);

        hotel1.Reservations.Add(reservation1);
        hotel1.Reservations.Add(reservation2);
        hotel2.Reservations.Add(reservation3);
        hotel3.Reservations.Add(reservation4);

        var payment1 = new Payment
        {
            Id = 1,
            Amount = 400,
            PaymentDate = new DateTime(2026, 3, 21),
            PaymentMethod = PaymentMethod.Card,
            IsPaid = true,
            Reservation = reservation1
        };

        var payment2 = new Payment
        {
            Id = 2,
            Amount = 120,
            PaymentDate = new DateTime(2026, 3, 23),
            PaymentMethod = PaymentMethod.Cash,
            IsPaid = false,
            Reservation = reservation2
        };

        var payment3 = new Payment
        {
            Id = 3,
            Amount = 900,
            PaymentDate = new DateTime(2026, 3, 26),
            PaymentMethod = PaymentMethod.Card,
            IsPaid = true,
            Reservation = reservation3
        };

        var payment4 = new Payment
        {
            Id = 4,
            Amount = 540,
            PaymentDate = new DateTime(2026, 3, 28),
            PaymentMethod = PaymentMethod.Cash,
            IsPaid = false,
            Reservation = reservation4
        };

        reservation1.Payments.Add(payment1);
        reservation2.Payments.Add(payment2);
        reservation3.Payments.Add(payment3);
        reservation4.Payments.Add(payment4);

        var review1 = new Review
        {
            Id = 1,
            Rating = 5,
            Comment = "Excellent service and clean rooms.",
            CreatedAt = new DateTime(2026, 3, 30),
            Guest = guest1,
            Hotel = hotel1
        };

        var review2 = new Review
        {
            Id = 2,
            Rating = 4,
            Comment = "Very nice spa experience.",
            CreatedAt = new DateTime(2026, 3, 29),
            Guest = guest3,
            Hotel = hotel2
        };

        guest1.Reviews.Add(review1);
        guest3.Reviews.Add(review2);

        Hotels = new List<Hotel> { hotel1, hotel2, hotel3 };
        Rooms = new List<Room> { room1, room2, room3, room4, room5, room6 };
        Services = new List<Service> { service1, service2, service3, service4 };
        Employees = new List<Employee> { employee1, employee2, employee3 };
        Guests = new List<Guest> { guest1, guest2, guest3 };
        Reservations = new List<Reservation> { reservation1, reservation2, reservation3, reservation4 };
        Payments = new List<Payment> { payment1, payment2, payment3, payment4 };
        Reviews = new List<Review> { review1, review2 };
    }

    public List<Hotel> GetAllHotels() => Hotels;
    public Hotel? GetHotelById(int id) => Hotels.SingleOrDefault(h => h.Id == id);

    public List<Room> GetAllRooms() => Rooms;
    public Room? GetRoomById(int id) => Rooms.SingleOrDefault(r => r.Id == id);
    public List<Room> GetRoomsByHotel(int hotelId) => Rooms.Where(r => r.HotelId == hotelId).ToList();

    public List<Service> GetAllServices() => Services;
    public Service? GetServiceById(int id) => Services.SingleOrDefault(s => s.Id == id);

    public List<Employee> GetAllEmployees() => Employees;
    public Employee? GetEmployeeById(int id) => Employees.SingleOrDefault(e => e.Id == id);

    public List<Guest> GetAllGuests() => Guests;
    public Guest? GetGuestById(int id) => Guests.SingleOrDefault(g => g.Id == id);

    public List<Reservation> GetAllReservations() => Reservations;
    public Reservation? GetReservationById(int id) => Reservations.SingleOrDefault(r => r.Id == id);

    public List<Payment> GetAllPayments() => Payments;
    public Payment? GetPaymentById(int id) => Payments.SingleOrDefault(p => p.Id == id);

    public List<Review> GetAllReviews() => Reviews;
    public Review? GetReviewById(int id) => Reviews.SingleOrDefault(r => r.Id == id);
}
