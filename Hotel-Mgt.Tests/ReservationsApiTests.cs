using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class ReservationsApiTests
{
    [Fact]
    public async Task Get_Reservations_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/reservations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Reservation_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/reservations/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Reservation_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/reservations", new
        {
            GuestId = 1,
            RoomId = 1,
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1),
            TotalPrice = 100
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
