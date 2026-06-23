using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class ReviewsApiTests
{
    [Fact]
    public async Task Get_Reviews_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/reviews");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Review_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/reviews/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Review_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/reviews", new
        {
            HotelId = 1,
            GuestId = 1,
            Rating = 5,
            Comment = "Great!",
            ReviewDate = DateTime.UtcNow
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Review_WithAdminUser_ReturnsCreated()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        // 1. Create Hotel
        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        // 2. Create Guest
        var guestResponse = await client.PostAsJsonAsync("/api/guests", new { FirstName = "Ivan", LastName = "Ivic", Email = "ivan@test.com", PhoneNumber = "123", DateOfBirth = DateTime.UtcNow.AddYears(-30), DocumentNumber = "HR123" });
        var guest = await guestResponse.Content.ReadFromJsonAsync<GuestDto>();

        // 3. Create Review
        var response = await client.PostAsJsonAsync("/api/reviews", new
        {
            HotelId = hotel!.Id,
            GuestId = guest!.Id,
            Rating = 5,
            Comment = "Great!",
            ReviewDate = DateTime.UtcNow
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
