using System.ComponentModel.DataAnnotations;
using HotelMgt.Model.Entities;
using HotelMgt.Model.Enums;

namespace HotelMgt.Web.DTOs;

public record HotelDto(int Id, string Name, string Address, string City, int Rating, string PhoneNumber);
public record HotelCreateDto([Required] string Name, [Required] string Address, [Required] string City, [Range(1, 5)] int Rating, [Required] string PhoneNumber);
public record HotelUpdateDto(int Id, [Required] string Name, [Required] string Address, [Required] string City, [Range(1, 5)] int Rating, [Required] string PhoneNumber);

public record RoomDto(int Id, string RoomNumber, int Floor, int Capacity, decimal PricePerNight, RoomType RoomType, bool IsAvailable, int HotelId, string? HotelName);
public record RoomCreateDto([Required] string RoomNumber, [Range(0, 100)] int Floor, [Range(1, 20)] int Capacity, [Range(0, 100000)] decimal PricePerNight, RoomType RoomType, bool IsAvailable, [Range(1, int.MaxValue)] int HotelId);
public record RoomUpdateDto(int Id, [Required] string RoomNumber, [Range(0, 100)] int Floor, [Range(1, 20)] int Capacity, [Range(0, 100000)] decimal PricePerNight, RoomType RoomType, bool IsAvailable, [Range(1, int.MaxValue)] int HotelId);

public record ServiceDto(int Id, string Name, string Description, decimal Price, bool IsAvailable, int HotelId, string? HotelName);
public record ServiceCreateDto([Required] string Name, [Required] string Description, [Range(0, 100000)] decimal Price, bool IsAvailable, [Range(1, int.MaxValue)] int HotelId);
public record ServiceUpdateDto(int Id, [Required] string Name, [Required] string Description, [Range(0, 100000)] decimal Price, bool IsAvailable, [Range(1, int.MaxValue)] int HotelId);

public record EmployeeDto(int Id, string FirstName, string LastName, string Email, string PhoneNumber, decimal Salary, EmployeeRole Role, DateTime HireDate, int HotelId, string? HotelName);
public record EmployeeCreateDto([Required] string FirstName, [Required] string LastName, [Required, EmailAddress] string Email, [Required] string PhoneNumber, [Range(0, 10000000)] decimal Salary, EmployeeRole Role, DateTime HireDate, [Range(1, int.MaxValue)] int HotelId);
public record EmployeeUpdateDto(int Id, [Required] string FirstName, [Required] string LastName, [Required, EmailAddress] string Email, [Required] string PhoneNumber, [Range(0, 10000000)] decimal Salary, EmployeeRole Role, DateTime HireDate, [Range(1, int.MaxValue)] int HotelId);

public record GuestDto(int Id, string FirstName, string LastName, string Email, string PhoneNumber, DateTime DateOfBirth, string DocumentNumber);
public record GuestCreateDto([Required] string FirstName, [Required] string LastName, [Required, EmailAddress] string Email, [Required] string PhoneNumber, DateTime DateOfBirth, [Required] string DocumentNumber);
public record GuestUpdateDto(int Id, [Required] string FirstName, [Required] string LastName, [Required, EmailAddress] string Email, [Required] string PhoneNumber, DateTime DateOfBirth, [Required] string DocumentNumber);

public record ReservationDto(int Id, string ReservationCode, DateTime ReservationDate, DateTime CheckInDate, DateTime CheckOutDate, decimal TotalPrice, ReservationStatus Status, int GuestId, string? GuestName, int RoomId, string? RoomNumber);
public record ReservationCreateDto([Required] string ReservationCode, DateTime ReservationDate, DateTime CheckInDate, DateTime CheckOutDate, [Range(0, 10000000)] decimal TotalPrice, ReservationStatus Status, [Range(1, int.MaxValue)] int GuestId, [Range(1, int.MaxValue)] int RoomId);
public record ReservationUpdateDto(int Id, [Required] string ReservationCode, DateTime ReservationDate, DateTime CheckInDate, DateTime CheckOutDate, [Range(0, 10000000)] decimal TotalPrice, ReservationStatus Status, [Range(1, int.MaxValue)] int GuestId, [Range(1, int.MaxValue)] int RoomId);

public record PaymentDto(int Id, decimal Amount, DateTime PaymentDate, PaymentMethod PaymentMethod, bool IsPaid, int ReservationId, string? ReservationCode);
public record PaymentCreateDto([Range(0, 10000000)] decimal Amount, DateTime PaymentDate, PaymentMethod PaymentMethod, bool IsPaid, [Range(1, int.MaxValue)] int ReservationId);
public record PaymentUpdateDto(int Id, [Range(0, 10000000)] decimal Amount, DateTime PaymentDate, PaymentMethod PaymentMethod, bool IsPaid, [Range(1, int.MaxValue)] int ReservationId);

public record ReviewDto(int Id, int Rating, string Comment, DateTime CreatedAt, int GuestId, string? GuestName, int HotelId, string? HotelName);
public record ReviewCreateDto([Range(1, 5)] int Rating, [Required] string Comment, DateTime CreatedAt, [Range(1, int.MaxValue)] int GuestId, [Range(1, int.MaxValue)] int HotelId);
public record ReviewUpdateDto(int Id, [Range(1, 5)] int Rating, [Required] string Comment, DateTime CreatedAt, [Range(1, int.MaxValue)] int GuestId, [Range(1, int.MaxValue)] int HotelId);

public static class ApiDtoMapper
{
    public static HotelDto ToDto(Hotel entity) => new(entity.Id, entity.Name, entity.Address, entity.City, entity.Rating, entity.PhoneNumber);
    public static RoomDto ToDto(Room entity) => new(entity.Id, entity.RoomNumber, entity.Floor, entity.Capacity, entity.PricePerNight, entity.RoomType, entity.IsAvailable, entity.HotelId, entity.Hotel?.Name);
    public static ServiceDto ToDto(Service entity) => new(entity.Id, entity.Name, entity.Description, entity.Price, entity.IsAvailable, entity.HotelId, entity.Hotel?.Name);
    public static EmployeeDto ToDto(Employee entity) => new(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.PhoneNumber, entity.Salary, entity.Role, entity.HireDate, entity.HotelId, entity.Hotel?.Name);
    public static GuestDto ToDto(Guest entity) => new(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.PhoneNumber, entity.DateOfBirth, entity.DocumentNumber);
    public static ReservationDto ToDto(Reservation entity) => new(entity.Id, entity.ReservationCode, entity.ReservationDate, entity.CheckInDate, entity.CheckOutDate, entity.TotalPrice, entity.Status, entity.GuestId, entity.Guest != null ? $"{entity.Guest.FirstName} {entity.Guest.LastName}" : null, entity.RoomId, entity.Room?.RoomNumber);
    public static PaymentDto ToDto(Payment entity) => new(entity.Id, entity.Amount, entity.PaymentDate, entity.PaymentMethod, entity.IsPaid, entity.ReservationId, entity.Reservation?.ReservationCode);
    public static ReviewDto ToDto(Review entity) => new(entity.Id, entity.Rating, entity.Comment, entity.CreatedAt, entity.GuestId, entity.Guest != null ? $"{entity.Guest.FirstName} {entity.Guest.LastName}" : null, entity.HotelId, entity.Hotel?.Name);

    public static Hotel Apply(Hotel entity, HotelUpdateDto dto) { entity.Name = dto.Name; entity.Address = dto.Address; entity.City = dto.City; entity.Rating = dto.Rating; entity.PhoneNumber = dto.PhoneNumber; return entity; }
    public static Room Apply(Room entity, RoomUpdateDto dto) { entity.RoomNumber = dto.RoomNumber; entity.Floor = dto.Floor; entity.Capacity = dto.Capacity; entity.PricePerNight = dto.PricePerNight; entity.RoomType = dto.RoomType; entity.IsAvailable = dto.IsAvailable; entity.HotelId = dto.HotelId; return entity; }
    public static Service Apply(Service entity, ServiceUpdateDto dto) { entity.Name = dto.Name; entity.Description = dto.Description; entity.Price = dto.Price; entity.IsAvailable = dto.IsAvailable; entity.HotelId = dto.HotelId; return entity; }
    public static Employee Apply(Employee entity, EmployeeUpdateDto dto) { entity.FirstName = dto.FirstName; entity.LastName = dto.LastName; entity.Email = dto.Email; entity.PhoneNumber = dto.PhoneNumber; entity.Salary = dto.Salary; entity.Role = dto.Role; entity.HireDate = dto.HireDate; entity.HotelId = dto.HotelId; return entity; }
    public static Guest Apply(Guest entity, GuestUpdateDto dto) { entity.FirstName = dto.FirstName; entity.LastName = dto.LastName; entity.Email = dto.Email; entity.PhoneNumber = dto.PhoneNumber; entity.DateOfBirth = dto.DateOfBirth; entity.DocumentNumber = dto.DocumentNumber; return entity; }
    public static Reservation Apply(Reservation entity, ReservationUpdateDto dto) { entity.ReservationCode = dto.ReservationCode; entity.ReservationDate = dto.ReservationDate; entity.CheckInDate = dto.CheckInDate; entity.CheckOutDate = dto.CheckOutDate; entity.TotalPrice = dto.TotalPrice; entity.Status = dto.Status; entity.GuestId = dto.GuestId; entity.RoomId = dto.RoomId; return entity; }
    public static Payment Apply(Payment entity, PaymentUpdateDto dto) { entity.Amount = dto.Amount; entity.PaymentDate = dto.PaymentDate; entity.PaymentMethod = dto.PaymentMethod; entity.IsPaid = dto.IsPaid; entity.ReservationId = dto.ReservationId; return entity; }
    public static Review Apply(Review entity, ReviewUpdateDto dto) { entity.Rating = dto.Rating; entity.Comment = dto.Comment; entity.CreatedAt = dto.CreatedAt; entity.GuestId = dto.GuestId; entity.HotelId = dto.HotelId; return entity; }
}
