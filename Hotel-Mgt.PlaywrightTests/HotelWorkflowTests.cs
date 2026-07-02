using System.Diagnostics;
using Microsoft.Playwright;
using Xunit;

namespace HotelMgt.PlaywrightTests;

/// <summary>
/// End-to-end UI scenario driven by a real Chromium instance against a locally hosted
/// instance of Hotel-Mgt.Web (started as a subprocess, the same way you would run the app).
/// </summary>
public class HotelWorkflowTests : IAsyncLifetime
{
    private const string BaseUrl = "http://127.0.0.1:5078";
    private Process? _serverProcess;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task InitializeAsync()
    {
        var repoRoot = FindRepoRoot();
        var webProjectDir = Path.Combine(repoRoot, "Hotel-Mgt.Web");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{webProjectDir}\" --no-launch-profile",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = webProjectDir,
        };
        startInfo.EnvironmentVariables["ASPNETCORE_URLS"] = BaseUrl;
        startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";

        _serverProcess = Process.Start(startInfo);
        Assert.NotNull(_serverProcess);

        await WaitForServerReadyAsync();

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    public async Task DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
        _playwright?.Dispose();

        if (_serverProcess is { HasExited: false })
        {
            _serverProcess.Kill(entireProcessTree: true);
            _serverProcess.WaitForExit(5000);
        }
        _serverProcess?.Dispose();
    }

    [Fact]
    public async Task Admin_Can_Register_Promote_And_Manage_A_Hotel_EndToEnd()
    {
        Assert.NotNull(_browser);
        var page = await _browser!.NewPageAsync();
        var testEmail = $"testuser_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}@hotel.local";

        // Step 1: Navigate to registration page
        await page.GotoAsync($"{BaseUrl}/Account/Register");

        // Step 2: Fill out registration details
        await page.FillAsync("input[name='Email']", testEmail);
        await page.FillAsync("input[name='Password']", "TestPass123!");
        await page.FillAsync("input[name='ConfirmPassword']", "TestPass123!");
        await page.FillAsync("input[name='OIB']", "12345678901");

        // Step 3: Submit registration
        await page.ClickAsync("button[type='submit']");
        await page.WaitForTimeoutAsync(2000);

        // Step 4: Verify logged in
        var contentAfterRegister = await page.ContentAsync();
        Assert.True(
            contentAfterRegister.Contains("Signed in as") || contentAfterRegister.Contains("Logout"),
            "Not signed in after registration.");

        // Step 5: Log out of the new user and log in as the seeded Admin
        await page.ClickAsync("button:has-text('Logout')");
        await page.WaitForTimeoutAsync(1000);
        await page.GotoAsync($"{BaseUrl}/Account/Login");
        await page.FillAsync("input[name='Email']", "admin@hotelmanager.local");
        await page.FillAsync("input[name='Password']", "Admin123!");
        await page.ClickAsync("button[type='submit']");
        await page.WaitForTimeoutAsync(2000);

        // Step 6: Navigate to Setup as Admin
        await page.GotoAsync($"{BaseUrl}/Setup");

        // Step 7: Promote the newly registered user to Admin
        var userRow = page.Locator($"tr:has-text('{testEmail}')");
        await userRow.Locator("select[name='roleName']").SelectOptionAsync("Admin");
        await userRow.Locator("button:has-text('Save')").ClickAsync();
        await page.WaitForTimeoutAsync(2000);

        // Step 8: Log out Admin and log back in as the promoted user to pick up the new claims
        await page.ClickAsync("button:has-text('Logout')");
        await page.WaitForTimeoutAsync(1000);
        await page.GotoAsync($"{BaseUrl}/Account/Login");
        await page.FillAsync("input[name='Email']", testEmail);
        await page.FillAsync("input[name='Password']", "TestPass123!");
        await page.ClickAsync("button[type='submit']");
        await page.WaitForTimeoutAsync(2000);

        // Step 9: Create a new Hotel as Admin
        var hotelName = $"Playwright Grand Hotel {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        await page.GotoAsync($"{BaseUrl}/hoteli/create");
        await page.FillAsync("input[name='Name']", hotelName);
        await page.FillAsync("input[name='Address']", "Testing Lane 12");
        await page.FillAsync("input[name='City']", "Zagreb");
        await page.FillAsync("input[name='Rating']", "5");
        await page.FillAsync("input[name='PhoneNumber']", "098111222");
        await page.ClickAsync("form button:has-text('Save')");
        await page.WaitForTimeoutAsync(2000);

        // Step 10: Verify the hotel exists in the listing
        await page.GotoAsync($"{BaseUrl}/hoteli");
        var listingContent = await page.ContentAsync();
        Assert.True(listingContent.Contains(hotelName), "Hotel not found in index list.");

        // --- Bonus steps (beyond the required 10) ---

        // Step 11: Open the created hotel's details and edit it
        var hotelRow = page.Locator($"tr:has-text('{hotelName}')");
        await hotelRow.Locator("a:has-text('Details')").ClickAsync();
        await page.WaitForTimeoutAsync(1000);
        var detailsUrl = page.Url;
        var hotelId = detailsUrl.TrimEnd('/').Split('/').Last();

        var updatedHotelName = $"{hotelName} Deluxe";
        await page.GotoAsync($"{BaseUrl}/hoteli/edit/{hotelId}");
        await page.FillAsync("input[name='Name']", updatedHotelName);
        await page.ClickAsync("form button:has-text('Save')");
        await page.WaitForTimeoutAsync(2000);

        await page.GotoAsync($"{BaseUrl}/hoteli");
        var listingAfterEdit = await page.ContentAsync();
        Assert.True(listingAfterEdit.Contains(updatedHotelName), "Updated hotel name not found after edit.");

        // Step 12: Use the global search box to find the edited hotel
        await page.GotoAsync($"{BaseUrl}/");
        await page.FillAsync("#globalSearchInput", updatedHotelName);
        await page.WaitForTimeoutAsync(1000);
        var searchResults = await page.ContentAsync();
        Assert.True(searchResults.Contains(updatedHotelName), "Global search did not surface the updated hotel.");

        // Step 13: Delete the hotel and verify it is gone from the listing
        await page.GotoAsync($"{BaseUrl}/hoteli/delete/{hotelId}");
        await page.ClickAsync("form button:has-text('Delete')");
        await page.WaitForTimeoutAsync(2000);

        await page.GotoAsync($"{BaseUrl}/hoteli");
        var listingAfterDelete = await page.ContentAsync();
        Assert.False(listingAfterDelete.Contains(updatedHotelName), "Hotel still present after deletion.");
    }

    private static async Task WaitForServerReadyAsync()
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTime.UtcNow.AddSeconds(30);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/Account/Login");
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Server not ready yet
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"Web server did not become ready at {BaseUrl} within 30 seconds.");
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Hotel-Mgt.slnx")))
        {
            dir = dir.Parent;
        }

        if (dir == null)
        {
            throw new DirectoryNotFoundException("Could not locate repository root (Hotel-Mgt.slnx) from test output directory.");
        }

        return dir.FullName;
    }
}
