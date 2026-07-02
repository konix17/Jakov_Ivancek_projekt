using HotelMgt.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelMgt.Web.Controllers.Api;

[ApiController]
[Route("api/search")]
public class SearchApiController : ControllerBase
{
    private readonly HotelDbContext _dbContext;

    public SearchApiController(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchResultItem>>> Search([FromQuery] string? q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(new List<SearchResultItem>());
        }

        var results = new List<SearchResultItem>();
        var query = q.Trim().ToLower();

        // 1. Search Sidebar Menus / Pages
        var pages = new List<(string Name, string Url)>
        {
            ("Dashboard / Nadzorna ploča", "/"),
            ("Hoteli / Hotels", "/hoteli"),
            ("Sobe / Rooms", "/sobe"),
            ("Gosti / Guests", "/gosti"),
            ("Rezervacije / Reservations", "/rezervacije"),
            ("Usluge / Services", "/usluge"),
            ("Zaposlenici / Employees", "/zaposlenici"),
            ("Plaćanja / Payments", "/placanja"),
            ("Recenzije / Reviews", "/recenzije")
        };

        foreach (var page in pages)
        {
            if (page.Name.ToLower().Contains(query))
            {
                results.Add(new SearchResultItem("Stranica / Izbornik", page.Name, page.Url));
            }
        }

        // 2. Search Hotels
        var hotels = await _dbContext.Hotels
            .Where(h => h.Name.ToLower().Contains(query) || h.City.ToLower().Contains(query))
            .Take(5)
            .ToListAsync();
        foreach (var h in hotels)
        {
            results.Add(new SearchResultItem("Hotel", $"{h.Name} ({h.City})", $"/hoteli/{h.Id}"));
        }

        // 3. Search Rooms
        var rooms = await _dbContext.Rooms
            .Include(r => r.Hotel)
            .Where(r => r.RoomNumber.ToLower().Contains(query))
            .Take(5)
            .ToListAsync();
        foreach (var r in rooms)
        {
            results.Add(new SearchResultItem("Soba", $"Soba {r.RoomNumber} - {r.Hotel.Name}", $"/sobe/{r.Id}"));
        }

        // 4. Search Guests
        var guests = await _dbContext.Guests
            .Where(g => g.FirstName.ToLower().Contains(query) || g.LastName.ToLower().Contains(query) || g.Email.ToLower().Contains(query) || g.DocumentNumber.ToLower().Contains(query))
            .Take(5)
            .ToListAsync();
        foreach (var g in guests)
        {
            results.Add(new SearchResultItem("Gost", $"{g.FirstName} {g.LastName} ({g.Email})", $"/gosti/{g.Id}"));
        }

        // 5. Search Reservations
        var reservations = await _dbContext.Reservations
            .Include(r => r.Guest)
            .Where(r => r.ReservationCode.ToLower().Contains(query))
            .Take(5)
            .ToListAsync();
        foreach (var r in reservations)
        {
            results.Add(new SearchResultItem("Rezervacija", $"Rezervacija {r.ReservationCode} - {r.Guest.FirstName} {r.Guest.LastName}", $"/rezervacije/{r.Id}"));
        }

        // 6. Search Employees
        var employees = await _dbContext.Employees
            .Include(e => e.Hotel)
            .Where(e => e.FirstName.ToLower().Contains(query) || e.LastName.ToLower().Contains(query) || e.Email.ToLower().Contains(query))
            .Take(5)
            .ToListAsync();
        foreach (var e in employees)
        {
            results.Add(new SearchResultItem("Zaposlenik", $"{e.FirstName} {e.LastName} ({e.Hotel.Name})", $"/zaposlenici/{e.Id}"));
        }

        return Ok(results);
    }
}

public record SearchResultItem(string Category, string Title, string Url);
