using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class HotelApiTests
{
    [Fact]
    public async Task Get_Hotels_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/hotels");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Hotel_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/hotels/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Hotel_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/hotels", new
        {
            Name = "Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Hotel_WithAdminUser_ReturnsCreated()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/hotels", new
        {
            Name = "Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Put_Hotel_WithAdminUser_UpdatesExistingRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/hotels", new
        {
            Name = "Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<HotelDto>();
        Assert.NotNull(created);

        var updateResponse = await client.PutAsJsonAsync($"/api/hotels/{created.Id}", new
        {
            Id = created.Id,
            Name = "Updated Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/hotels/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<HotelDto>();

        Assert.Equal("Updated Bay Hotel", updated?.Name);
    }

    [Fact]
    public async Task Delete_Hotel_WithAdminUser_RemovesRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/hotels", new
        {
            Name = "Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<HotelDto>();
        Assert.NotNull(created);

        var deleteResponse = await client.DeleteAsync($"/api/hotels/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/hotels/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_Hotel_WithInvalidData_ReturnsBadRequest()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/hotels", new
        {
            Name = "", // Required
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 10, // Invalid range (1-5)
            PhoneNumber = "012345678"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Hotel_ReturnsNotFound_WhenMissing()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/hotels/9999", new
        {
            Id = 9999,
            Name = "Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_Hotel_ReturnsBadRequest_WhenIdMismatch()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/hotels/1", new
        {
            Id = 2, // Id mismatch with route id (1)
            Name = "Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}