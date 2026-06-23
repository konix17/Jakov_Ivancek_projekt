using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class ServicesApiTests
{
    [Fact]
    public async Task Get_Services_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/services");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Service_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/services/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Service_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/services", new
        {
            Name = "Spa",
            Description = "Relaxing spa",
            Price = 50,
            HotelId = 1
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Service_WithAdminUser_ReturnsCreated()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        // 1. Create Hotel
        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        // 2. Create Service
        var response = await client.PostAsJsonAsync("/api/services", new
        {
            Name = "Spa",
            Description = "Relaxing spa",
            Price = 50,
            HotelId = hotel!.Id
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
