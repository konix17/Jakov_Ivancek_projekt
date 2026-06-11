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
            PaymentMethod = "Credit Card"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
