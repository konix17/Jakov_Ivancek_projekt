using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class GuestsApiTests
{
    [Fact]
    public async Task Get_Guests_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/guests");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Guest_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/guests/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Guest_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/guests", new
        {
            FirstName = "Guest",
            LastName = "One",
            Email = "guest@example.com",
            PhoneNumber = "123",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            DocumentNumber = "DOC123"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Guest_WithAdminUser_ReturnsCreated()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/guests", new
        {
            FirstName = "Guest",
            LastName = "One",
            Email = "guest@example.com",
            PhoneNumber = "123",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            DocumentNumber = "DOC123"
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Put_Guest_WithAdminUser_UpdatesExistingRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/guests", new
        {
            FirstName = "Guest",
            LastName = "One",
            Email = "guest@example.com",
            PhoneNumber = "123",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            DocumentNumber = "DOC123"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<GuestDto>();

        var updateResponse = await client.PutAsJsonAsync($"/api/guests/{created!.Id}", new
        {
            Id = created.Id,
            FirstName = "GuestTwo",
            LastName = "One",
            Email = "guest@example.com",
            PhoneNumber = "123",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            DocumentNumber = "DOC123"
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/guests/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<GuestDto>();
        Assert.Equal("GuestTwo", updated?.FirstName);
    }

    [Fact]
    public async Task Delete_Guest_WithAdminUser_RemovesRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/guests", new
        {
            FirstName = "Guest",
            LastName = "One",
            Email = "guest@example.com",
            PhoneNumber = "123",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            DocumentNumber = "DOC123"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<GuestDto>();
        var deleteResponse = await client.DeleteAsync($"/api/guests/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/guests/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_Guest_WithInvalidEmail_ReturnsBadRequest()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/guests", new
        {
            FirstName = "Guest",
            LastName = "One",
            Email = "not-an-email", // invalid email format
            PhoneNumber = "123",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            DocumentNumber = "DOC123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Guest_ReturnsNotFound_WhenMissing()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/guests/9999", new
        {
            Id = 9999,
            FirstName = "Guest",
            LastName = "One",
            Email = "guest@example.com",
            PhoneNumber = "123",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            DocumentNumber = "DOC123"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
