using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class EmployeesApiTests
{
    [Fact]
    public async Task Get_Employees_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/employees");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Employee_ReturnsNotFound_WhenMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/employees/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Employee_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/employees", new
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123",
            Salary = 1000,
            Role = 2,
            HireDate = DateTime.UtcNow,
            HotelId = 1
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Employee_WithAdminUser_ReturnsCreated()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();
        
        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        var response = await client.PostAsJsonAsync("/api/employees", new
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123",
            Salary = 1000,
            Role = 2,
            HireDate = DateTime.UtcNow,
            HotelId = hotel!.Id
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Put_Employee_WithAdminUser_UpdatesExistingRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        var createResponse = await client.PostAsJsonAsync("/api/employees", new
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123",
            Salary = 1000,
            Role = 2,
            HireDate = DateTime.UtcNow,
            HotelId = hotel!.Id
        });

        var created = await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();

        var updateResponse = await client.PutAsJsonAsync($"/api/employees/{created!.Id}", new
        {
            Id = created.Id,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123",
            Salary = 1000,
            Role = 2,
            HireDate = DateTime.UtcNow,
            HotelId = hotel.Id
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/employees/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.Equal("Jane", updated?.FirstName);
    }

    [Fact]
    public async Task Delete_Employee_WithAdminUser_RemovesRecord()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        var createResponse = await client.PostAsJsonAsync("/api/employees", new
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123",
            Salary = 1000,
            Role = 2,
            HireDate = DateTime.UtcNow,
            HotelId = hotel!.Id
        });

        var created = await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();
        var deleteResponse = await client.DeleteAsync($"/api/employees/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/employees/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
