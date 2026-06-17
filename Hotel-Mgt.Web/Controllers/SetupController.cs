using HotelMgt.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelMgt.Web.Controllers;

public class UserRolesViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

[Authorize(Roles = "Admin")]
public class SetupController : Controller
{
    private readonly HotelDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<HotelMgt.Model.Entities.AppUser> _userManager;

    public SetupController(HotelDbContext context, RoleManager<IdentityRole> roleManager, UserManager<HotelMgt.Model.Entities.AppUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Setup";
        ViewData["Description"] = "Reset demo data, ensure identity roles, or manage user permissions.";

        var users = await _userManager.Users.ToListAsync();
        var userRolesList = new List<UserRolesViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRolesList.Add(new UserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? user.UserName ?? "No Email",
                Roles = roles.ToList()
            });
        }

        return View(userRolesList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Korisnik nije pronađen.";
            return RedirectToAction(nameof(Index));
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == userId && roleName != "Admin")
        {
            TempData["ErrorMessage"] = "Ne možete maknuti ulogu Admin sami sebi.";
            return RedirectToAction(nameof(Index));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            TempData["ErrorMessage"] = "Greška pri brisanju starih uloga.";
            return RedirectToAction(nameof(Index));
        }

        if (!string.IsNullOrEmpty(roleName))
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            var addResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Greška pri dodavanju nove uloge.";
                return RedirectToAction(nameof(Index));
            }
        }

        TempData["Message"] = $"Uloga za korisnika {user.Email} uspješno promijenjena u {roleName}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetDatabase()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        TempData["Message"] = "The database was reset and the identity roles were recreated.";
        return RedirectToAction(nameof(Index));
    }

}
