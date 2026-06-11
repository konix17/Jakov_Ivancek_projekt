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
}
