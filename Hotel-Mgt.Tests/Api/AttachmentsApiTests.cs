using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HotelMgt.Web.DTOs;
using HotelMgt.Web.Controllers.Api;
using Xunit;

namespace HotelMgt.Tests;

public class AttachmentsApiTests
{
    [Fact]
    public async Task Get_Attachments_ReturnsSuccessStatusCode()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/hotels/1/attachments");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Upload_Attachment_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        content.Add(fileContent, "file", "test.png");

        var response = await client.PostAsync("/api/hotels/1/attachments", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Upload_Attachment_WithAdminUser_ReturnsOk()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        using var client = factory.CreateClient();

        // Ensure hotel exists
        var hotelResponse = await client.PostAsJsonAsync("/api/hotels", new { Name = "Test Hotel", Address = "A", City = "C", Rating = 5, PhoneNumber = "1" });
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<HotelDto>();

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        content.Add(fileContent, "file", "test.png");

        var response = await client.PostAsync($"/api/hotels/{hotel!.Id}/attachments", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
