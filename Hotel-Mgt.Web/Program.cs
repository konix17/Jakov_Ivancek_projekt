using HotelMgt.Model;
using HotelMgt.Model.Entities;
using HotelMgt.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

var sqliteConnectionString = new SqliteConnectionStringBuilder(builder.Configuration.GetConnectionString("HotelDb") ?? "Data Source=HotelDb.db")
{
    Mode = SqliteOpenMode.ReadWriteCreate,
    ForeignKeys = true,
    Cache = SqliteCacheMode.Shared
}.ToString();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));
builder.Services.AddScoped<IHotelRepository, EfHotelRepository>();
builder.Services.AddScoped<HotelMgt.Web.Services.IAttachmentService, HotelMgt.Web.Services.AttachmentService>();

var authenticationBuilder = builder.Services.AddAuthentication();

if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authenticationBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    });
}

if (!string.IsNullOrWhiteSpace(facebookAppId) && !string.IsNullOrWhiteSpace(facebookAppSecret))
{
    authenticationBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId;
        options.AppSecret = facebookAppSecret;
    });
}

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<HotelDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.UseAuthentication();

var supportedCultures = new[]
{
    new CultureInfo("hr"),
    new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("hr"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

    var sqliteBuilder = new SqliteConnectionStringBuilder(sqliteConnectionString);
    var dbPath = sqliteBuilder.DataSource;
    if (!Path.IsPathRooted(dbPath))
    {
        dbPath = Path.Combine(app.Environment.ContentRootPath, dbPath);
    }

    await using var resetConnection = new SqliteConnection(sqliteConnectionString);
    await resetConnection.OpenAsync();
    await using var resetCommand = resetConnection.CreateCommand();
    resetCommand.CommandText = "PRAGMA foreign_keys=ON;";
    await resetCommand.ExecuteNonQueryAsync();

    await db.Database.EnsureCreatedAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    foreach (var roleName in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    const string demoAdminEmail = "admin@hotelmanager.local";
    const string demoAdminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(demoAdminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = demoAdminEmail,
            Email = demoAdminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, demoAdminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program { }
