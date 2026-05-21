using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HotelMgt.Model.Entities;
using HotelMgt.Model.Enums;

namespace HotelMgt.Web.Models
{
    public class HotelFormModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public static HotelFormModel FromEntity(Hotel hotel)
            => new HotelFormModel
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Rating = hotel.Rating,
                PhoneNumber = hotel.PhoneNumber
            };

        public void UpdateEntity(Hotel hotel)
        {
            hotel.Name = Name;
            hotel.Address = Address;
            hotel.City = City;
            hotel.Rating = Rating;
            hotel.PhoneNumber = PhoneNumber;
        }
    }

    public class RoomFormModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomNumber { get; set; } = string.Empty;

        [Range(0, 100)]
        public int Floor { get; set; }

        [Range(1, 20)]
        public int Capacity { get; set; }

        [Range(0.01, 10000)]
        public decimal PricePerNight { get; set; }

        [Required]
        public RoomType RoomType { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int HotelId { get; set; }

        public static RoomFormModel FromEntity(Room room)
            => new RoomFormModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                Capacity = room.Capacity,
                PricePerNight = room.PricePerNight,
                RoomType = room.RoomType,
                IsAvailable = room.IsAvailable,
                HotelId = room.HotelId
            };

        public void UpdateEntity(Room room)
        {
            room.RoomNumber = RoomNumber;
            room.Floor = Floor;
            room.Capacity = Capacity;
            room.PricePerNight = PricePerNight;
            room.RoomType = RoomType;
            room.IsAvailable = IsAvailable;
            room.HotelId = HotelId;
        }
    }

    public class ServiceFormModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(0.0, 10000.0)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int HotelId { get; set; }

        public static ServiceFormModel FromEntity(Service service)
            => new ServiceFormModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                IsAvailable = service.IsAvailable,
                HotelId = service.HotelId
            };

        public void UpdateEntity(Service service)
        {
            service.Name = Name;
            service.Description = Description;
            service.Price = Price;
            service.IsAvailable = IsAvailable;
            service.HotelId = HotelId;
        }
    }

    public class EmployeeFormModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range(0.0, 1000000.0)]
        public decimal Salary { get; set; }

        [Required]
        public EmployeeRole Role { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        public int HotelId { get; set; }

        public static EmployeeFormModel FromEntity(Employee employee)
            => new EmployeeFormModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Salary = employee.Salary,
                Role = employee.Role,
                HireDate = employee.HireDate,
                HotelId = employee.HotelId
            };

        public void UpdateEntity(Employee employee)
        {
            employee.FirstName = FirstName;
            employee.LastName = LastName;
            employee.Email = Email;
            employee.PhoneNumber = PhoneNumber;
            employee.Salary = Salary;
            employee.Role = Role;
            employee.HireDate = HireDate;
            employee.HotelId = HotelId;
        }
    }

    public class GuestFormModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;

        public static GuestFormModel FromEntity(Guest guest)
            => new GuestFormModel
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Email = guest.Email,
                PhoneNumber = guest.PhoneNumber,
                DateOfBirth = guest.DateOfBirth,
                DocumentNumber = guest.DocumentNumber
            };

        public void UpdateEntity(Guest guest)
        {
            guest.FirstName = FirstName;
            guest.LastName = LastName;
            guest.Email = Email;
            guest.PhoneNumber = PhoneNumber;
            guest.DateOfBirth = DateOfBirth;
            guest.DocumentNumber = DocumentNumber;
        }
    }

    public class ReservationFormModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string ReservationCode { get; set; } = string.Empty;

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Range(0.0, 1000000.0)]
        public decimal TotalPrice { get; set; }

        [Required]
        public ReservationStatus Status { get; set; }

        [Required]
        public int GuestId { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        public int RoomId { get; set; }

        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        public static ReservationFormModel FromEntity(Reservation reservation)
            => new ReservationFormModel
            {
                Id = reservation.Id,
                ReservationCode = reservation.ReservationCode,
                ReservationDate = reservation.ReservationDate,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                TotalPrice = reservation.TotalPrice,
                Status = reservation.Status,
                GuestId = reservation.GuestId,
                HotelId = reservation.Room?.HotelId ?? 0,
                RoomId = reservation.RoomId,
                SelectedServiceIds = reservation.Services?.Select(s => s.Id).ToList() ?? new List<int>()
            };

        public void UpdateEntity(Reservation reservation, IEnumerable<Service> services)
        {
            reservation.ReservationCode = ReservationCode;
            reservation.ReservationDate = ReservationDate;
            reservation.CheckInDate = CheckInDate;
            reservation.CheckOutDate = CheckOutDate;
            reservation.TotalPrice = TotalPrice;
            reservation.Status = Status;
            reservation.GuestId = GuestId;
            reservation.RoomId = RoomId;
            reservation.Services ??= new List<Service>();
            reservation.Services.Clear();
            foreach (var serviceId in SelectedServiceIds)
            {
                reservation.Services.Add(new Service { Id = serviceId });
            }
        }
    }

    public class PaymentFormModel
    {
        public int Id { get; set; }

        [Range(0.0, 1000000.0)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public bool IsPaid { get; set; }

        [Required]
        public int ReservationId { get; set; }

        public static PaymentFormModel FromEntity(Payment payment)
            => new PaymentFormModel
            {
                Id = payment.Id,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                IsPaid = payment.IsPaid,
                ReservationId = payment.ReservationId
            };

        public void UpdateEntity(Payment payment)
        {
            payment.Amount = Amount;
            payment.PaymentDate = PaymentDate;
            payment.PaymentMethod = PaymentMethod;
            payment.IsPaid = IsPaid;
            payment.ReservationId = ReservationId;
        }
    }

    public class ReviewFormModel
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int GuestId { get; set; }

        [Required]
        public int HotelId { get; set; }

        public static ReviewFormModel FromEntity(Review review)
            => new ReviewFormModel
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                GuestId = review.GuestId,
                HotelId = review.HotelId
            };

        public void UpdateEntity(Review review)
        {
            review.Rating = Rating;
            review.Comment = Comment;
            review.CreatedAt = CreatedAt;
            review.GuestId = GuestId;
            review.HotelId = HotelId;
        }
    }
}
