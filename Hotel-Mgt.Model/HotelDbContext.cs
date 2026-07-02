using HotelMgt.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelMgt.Model;

public class HotelDbContext : IdentityDbContext<AppUser>
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Attachment> Attachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Service>()
            .HasOne(s => s.Hotel)
            .WithMany(h => h.Services)
            .HasForeignKey(s => s.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Hotel)
            .WithMany(h => h.Employees)
            .HasForeignKey(e => e.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Guest)
            .WithMany(g => g.Reservations)
            .HasForeignKey(r => r.GuestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Room)
            .WithMany(room => room.Reservations)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Reservation)
            .WithMany(r => r.Payments)
            .HasForeignKey(p => p.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Guest)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GuestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Reviews)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Hotel)
            .WithMany(h => h.Attachments)
            .HasForeignKey(a => a.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reservation>()
            .HasMany(r => r.Services)
            .WithMany(s => s.Reservations);

        modelBuilder.Entity<Employee>()
            .Property(e => e.Salary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Reservation>()
            .Property(r => r.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Room>()
            .Property(r => r.PricePerNight)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Service>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);

        // Seed data
        modelBuilder.Entity<Hotel>().HasData(
            new Hotel { Id = 1, Name = "Adriatic Hotel", Address = "Ilica 10", City = "Zagreb", Rating = 4, PhoneNumber = "0911111111" },
            new Hotel { Id = 2, Name = "Sunset Resort", Address = "Riva 20", City = "Split", Rating = 5, PhoneNumber = "0922222222" },
            new Hotel { Id = 3, Name = "Mountain View Hotel", Address = "Korzo 5", City = "Rijeka", Rating = 3, PhoneNumber = "0933333333" }
        );

        modelBuilder.Entity<Room>().HasData(
            new { Id = 1, RoomNumber = "101", Floor = 1, Capacity = 1, PricePerNight = 80m, RoomType = HotelMgt.Model.Enums.RoomType.Single, IsAvailable = true, HotelId = 1 },
            new { Id = 2, RoomNumber = "102", Floor = 1, Capacity = 2, PricePerNight = 120m, RoomType = HotelMgt.Model.Enums.RoomType.Double, IsAvailable = true, HotelId = 1 },
            new { Id = 3, RoomNumber = "201", Floor = 2, Capacity = 3, PricePerNight = 250m, RoomType = HotelMgt.Model.Enums.RoomType.Suite, IsAvailable = false, HotelId = 2 },
            new { Id = 4, RoomNumber = "202", Floor = 2, Capacity = 2, PricePerNight = 150m, RoomType = HotelMgt.Model.Enums.RoomType.Double, IsAvailable = true, HotelId = 2 },
            new { Id = 5, RoomNumber = "301", Floor = 3, Capacity = 1, PricePerNight = 70m, RoomType = HotelMgt.Model.Enums.RoomType.Single, IsAvailable = true, HotelId = 3 },
            new { Id = 6, RoomNumber = "302", Floor = 3, Capacity = 3, PricePerNight = 180m, RoomType = HotelMgt.Model.Enums.RoomType.Suite, IsAvailable = false, HotelId = 3 }
        );

        modelBuilder.Entity<Service>().HasData(
            new { Id = 1, Name = "Breakfast", Description = "Buffet breakfast", Price = 15m, IsAvailable = true, HotelId = 1 },
            new { Id = 2, Name = "Parking", Description = "Private hotel parking", Price = 10m, IsAvailable = true, HotelId = 1 },
            new { Id = 3, Name = "Spa", Description = "Spa and wellness access", Price = 30m, IsAvailable = true, HotelId = 2 },
            new { Id = 4, Name = "Dinner", Description = "Hotel restaurant dinner", Price = 25m, IsAvailable = true, HotelId = 3 }
        );

        modelBuilder.Entity<Employee>().HasData(
            new { Id = 1, FirstName = "Marko", LastName = "Maric", Email = "marko@adriatic.hr", PhoneNumber = "0991111111", Salary = 1200m, Role = HotelMgt.Model.Enums.EmployeeRole.Manager, HireDate = new DateTime(2022, 5, 10), HotelId = 1 },
            new { Id = 2, FirstName = "Ivana", LastName = "Ivic", Email = "ivana@sunset.hr", PhoneNumber = "0992222222", Salary = 1000m, Role = HotelMgt.Model.Enums.EmployeeRole.Receptionist, HireDate = new DateTime(2023, 2, 15), HotelId = 2 },
            new { Id = 3, FirstName = "Petra", LastName = "Peric", Email = "petra@mountain.hr", PhoneNumber = "0993333333", Salary = 900m, Role = HotelMgt.Model.Enums.EmployeeRole.Housekeeping, HireDate = new DateTime(2024, 1, 20), HotelId = 3 }
        );

        modelBuilder.Entity<Guest>().HasData(
            new { Id = 1, FirstName = "Ivan", LastName = "Ivic", Email = "ivan@gmail.com", PhoneNumber = "0951111111", DateOfBirth = new DateTime(1999, 5, 10), DocumentNumber = "HR12345" },
            new { Id = 2, FirstName = "Ana", LastName = "Anic", Email = "ana@gmail.com", PhoneNumber = "0952222222", DateOfBirth = new DateTime(2000, 8, 15), DocumentNumber = "HR67890" },
            new { Id = 3, FirstName = "Luka", LastName = "Lukic", Email = "luka@gmail.com", PhoneNumber = "0953333333", DateOfBirth = new DateTime(1998, 3, 22), DocumentNumber = "HR54321" }
        );

        modelBuilder.Entity<Reservation>().HasData(
            new { Id = 1, ReservationCode = "RES001", ReservationDate = new DateTime(2026, 3, 20), CheckInDate = new DateTime(2026, 4, 10), CheckOutDate = new DateTime(2026, 4, 15), TotalPrice = 400m, Status = HotelMgt.Model.Enums.ReservationStatus.Confirmed, GuestId = 1, RoomId = 1 },
            new { Id = 2, ReservationCode = "RES002", ReservationDate = new DateTime(2026, 3, 21), CheckInDate = new DateTime(2026, 4, 12), CheckOutDate = new DateTime(2026, 4, 18), TotalPrice = 600m, Status = HotelMgt.Model.Enums.ReservationStatus.Confirmed, GuestId = 2, RoomId = 2 },
            new { Id = 3, ReservationCode = "RES003", ReservationDate = new DateTime(2026, 3, 22), CheckInDate = new DateTime(2026, 4, 14), CheckOutDate = new DateTime(2026, 4, 20), TotalPrice = 750m, Status = HotelMgt.Model.Enums.ReservationStatus.Pending, GuestId = 3, RoomId = 3 },
            new { Id = 4, ReservationCode = "RES004", ReservationDate = new DateTime(2026, 3, 23), CheckInDate = new DateTime(2026, 4, 16), CheckOutDate = new DateTime(2026, 4, 22), TotalPrice = 540m, Status = HotelMgt.Model.Enums.ReservationStatus.Cancelled, GuestId = 1, RoomId = 4 }
        );

        modelBuilder.Entity<Payment>().HasData(
            new { Id = 1, Amount = 400m, PaymentDate = new DateTime(2026, 3, 20), PaymentMethod = HotelMgt.Model.Enums.PaymentMethod.Card, IsPaid = true, ReservationId = 1 },
            new { Id = 2, Amount = 600m, PaymentDate = new DateTime(2026, 3, 21), PaymentMethod = HotelMgt.Model.Enums.PaymentMethod.Cash, IsPaid = true, ReservationId = 2 },
            new { Id = 3, Amount = 750m, PaymentDate = new DateTime(2026, 3, 26), PaymentMethod = HotelMgt.Model.Enums.PaymentMethod.Card, IsPaid = false, ReservationId = 3 },
            new { Id = 4, Amount = 540m, PaymentDate = new DateTime(2026, 3, 28), PaymentMethod = HotelMgt.Model.Enums.PaymentMethod.Cash, IsPaid = false, ReservationId = 4 }
        );

        modelBuilder.Entity<Review>().HasData(
            new { Id = 1, Rating = 5, Comment = "Excellent service and clean rooms.", CreatedAt = new DateTime(2026, 3, 30), GuestId = 1, HotelId = 1 },
            new { Id = 2, Rating = 4, Comment = "Very nice spa experience.", CreatedAt = new DateTime(2026, 3, 29), GuestId = 3, HotelId = 2 }
        );
    }
}
