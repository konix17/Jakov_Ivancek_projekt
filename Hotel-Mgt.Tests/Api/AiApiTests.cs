using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HotelMgt.Tests;

public class AiApiTests
{
    // Forces the fallback rules-based parser regardless of any Gemini API key configured
    // on the machine (e.g. via dotnet user-secrets), so these tests stay deterministic and offline.
    private static WebApplicationFactory<Program> CreateFactoryWithoutGeminiKey()
    {
        var factory = new AuthenticatedWebApplicationFactory();
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Gemini:ApiKey"] = ""
                });
            });
        });
    }

    [Fact]
    public async Task Post_ParseQuery_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/parse", new { Query = "Dodaj gosta Ivan Horvat" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_ParseQuery_WithEmptyQuery_ReturnsBadRequest()
    {
        using var factory = CreateFactoryWithoutGeminiKey();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/parse", new { Query = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ParseQuery_CreateGuestFromNaturalLanguage_CreatesGuest()
    {
        using var factory = CreateFactoryWithoutGeminiKey();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/parse", new
        {
            Query = "Dodaj gosta imenom Ivan Horvat, email ivan.horvat@test.com"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Kreiran gost", body);

        var guestsResponse = await client.GetAsync("/api/guests");
        var guestsContent = await guestsResponse.Content.ReadAsStringAsync();
        Assert.Contains("ivan.horvat@test.com", guestsContent);
    }

    [Fact]
    public async Task Post_ParseQuery_CreateHotelFromNaturalLanguage_CreatesHotel()
    {
        using var factory = CreateFactoryWithoutGeminiKey();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/parse", new
        {
            Query = "Kreiraj hotel Adria, grad Split, ocjena 5"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Kreiran hotel", body);

        var hotelsResponse = await client.GetAsync("/api/hotels");
        var hotelsContent = await hotelsResponse.Content.ReadAsStringAsync();
        Assert.Contains("Split", hotelsContent);
    }

    [Fact]
    public async Task Post_ParseQuery_WithUnrecognizedText_ReturnsBadRequest()
    {
        using var factory = CreateFactoryWithoutGeminiKey();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/parse", new
        {
            Query = "asdkjqwhe qwoiuhdqwoiuh not a recognizable command"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
