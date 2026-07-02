using System.Net;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using Xunit;

namespace HotelMgt.Tests;

public class SearchApiTests
{
    [Fact]
    public async Task Get_Search_WithEmptyQuery_ReturnsEmptyList()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/search?q=");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var results = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task Get_Search_MatchesStaticPageByName()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/search?q=Hoteli");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Hoteli", content);
    }

    [Fact]
    public async Task Get_Search_MatchesCreatedHotelByName()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/hotels", new
        {
            Name = "Searchable Bay Hotel",
            Address = "Main Street 1",
            City = "Zagreb",
            Rating = 5,
            PhoneNumber = "012345678"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<HotelDto>();
        Assert.NotNull(created);

        var response = await client.GetAsync("/api/search?q=Searchable Bay");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Searchable Bay Hotel", content);
        Assert.Contains($"/hoteli/{created!.Id}", content);
    }

    [Fact]
    public async Task Get_Search_WithNoMatches_ReturnsEmptyResultsOnly()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/search?q=zzzznonexistentzzzz");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var results = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(results);
        Assert.Empty(results);
    }
}
