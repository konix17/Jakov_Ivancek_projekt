using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class PaymentsApiTests
{
    [Fact]
    public async Task Get_Payments_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/payments");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Payment_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/payments/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Payment_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/payments", new
        {
            ReservationId = 1,
            Amount = 100,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = 1,
            IsPaid = true
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Payment_WithAdminUser_ReturnsCreated()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        // 1. Create Hotel
        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        // 2. Create Room
        var roomResponse = await client.PostAsJsonAsync("/api/rooms", new { RoomNumber = "999", Floor = 1, Capacity = 2, PricePerNight = 100, RoomType = 1, IsAvailable = true, HotelId = hotel!.Id });
        var room = await roomResponse.Content.ReadFromJsonAsync<RoomDto>();

        // 3. Create Guest
        var guestResponse = await client.PostAsJsonAsync("/api/guests", new { FirstName = "Ivan", LastName = "Ivic", Email = "ivan@test.com", PhoneNumber = "123", DateOfBirth = DateTime.UtcNow.AddYears(-30), DocumentNumber = "HR123" });
        var guest = await guestResponse.Content.ReadFromJsonAsync<GuestDto>();

        // 4. Create Reservation
        var resResponse = await client.PostAsJsonAsync("/api/reservations", new
        {
            ReservationCode = "RES123",
            ReservationDate = DateTime.UtcNow,
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1),
            TotalPrice = 100,
            Status = 1,
            GuestId = guest!.Id,
            RoomId = room!.Id
        });
        var res = await resResponse.Content.ReadFromJsonAsync<ReservationDto>();

        // 5. Create Payment
        var response = await client.PostAsJsonAsync("/api/payments", new
        {
            ReservationId = res!.Id,
            Amount = 100,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = 1,
            IsPaid = true
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
