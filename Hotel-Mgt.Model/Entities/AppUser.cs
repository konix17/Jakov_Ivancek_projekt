using Microsoft.AspNetCore.Identity;

namespace HotelMgt.Model.Entities;

public class AppUser : IdentityUser
{
    public string? OIB { get; set; }
}
