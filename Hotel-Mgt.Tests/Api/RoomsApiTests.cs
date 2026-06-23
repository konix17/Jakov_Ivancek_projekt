using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class RoomsApiTests
{
    [Fact]
    public async Task Get_Rooms_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/rooms");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Room_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/rooms/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Room_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/rooms", new
        {
            RoomNumber = "101",
            Floor = 1,
            Capacity = 2,
            PricePerNight = 100,
            RoomType = 1,
            IsAvailable = true,
            HotelId = 1
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Room_WithAdminUser_ReturnsCreated()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();
        
        // First create a hotel
        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        var response = await client.PostAsJsonAsync("/api/rooms", new
        {
            RoomNumber = "101",
            Floor = 1,
            Capacity = 2,
            PricePerNight = 100,
            RoomType = 1,
            IsAvailable = true,
            HotelId = hotel!.Id
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Put_Room_WithAdminUser_UpdatesExistingRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        var createResponse = await client.PostAsJsonAsync("/api/rooms", new
        {
            RoomNumber = "101",
            Floor = 1,
            Capacity = 2,
            PricePerNight = 100,
            RoomType = 1,
            IsAvailable = true,
            HotelId = hotel!.Id
        });

        var created = await createResponse.Content.ReadFromJsonAsync<RoomDto>();

        var updateResponse = await client.PutAsJsonAsync($"/api/rooms/{created!.Id}", new
        {
            Id = created.Id,
            RoomNumber = "102",
            Floor = 1,
            Capacity = 2,
            PricePerNight = 100,
            RoomType = 1,
            IsAvailable = true,
            HotelId = hotel.Id
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/rooms/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<RoomDto>();
        Assert.Equal("102", updated?.RoomNumber);
    }

    [Fact]
    public async Task Delete_Room_WithAdminUser_RemovesRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        var createResponse = await client.PostAsJsonAsync("/api/rooms", new
        {
            RoomNumber = "101",
            Floor = 1,
            Capacity = 2,
            PricePerNight = 100,
            RoomType = 1,
            IsAvailable = true,
            HotelId = hotel!.Id
        });

        var created = await createResponse.Content.ReadFromJsonAsync<RoomDto>();
        var deleteResponse = await client.DeleteAsync($"/api/rooms/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/rooms/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
