using HotelMgt.Model;
using HotelMgt.Model.Entities;
using HotelMgt.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;


namespace HotelMgt.Web.Controllers.Api;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/ai")]
public class AiApiController : ControllerBase
{
    private readonly HotelDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private static readonly HttpClient _httpClient = new HttpClient();

    public AiApiController(HotelDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public class ParseRequest
    {
        public string Query { get; set; } = string.Empty;
    }

    [HttpPost("parse")]
    public async Task<IActionResult> ParseQuery([FromBody] ParseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Upit ne može biti prazan.");
        }

        string resultJson;
        var apiKey = _configuration["Gemini:ApiKey"];

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            try
            {
                resultJson = await CallGeminiApiAsync(request.Query, apiKey);
            }
            catch (Exception)
            {
                // Fallback to rules-based parser on API error
                resultJson = ParseUsingFallbackRules(request.Query);
            }
        }
        else
        {
            // Fallback to rules-based parser if no API key is provided
            resultJson = ParseUsingFallbackRules(request.Query);
        }

        // Process the parsed action JSON
        try
        {
            using var doc = JsonDocument.Parse(resultJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("action", out var actionProp))
            {
                return BadRequest(new { success = false, message = "AI nije uspio prepoznati akciju u upitu.", rawJson = resultJson });
            }

            string action = actionProp.GetString() ?? "";
            var data = root.GetProperty("data");

            if (action == "CreateGuest")
            {
                var guest = new Guest
                {
                    FirstName = data.GetProperty("FirstName").GetString() ?? "Novi",
                    LastName = data.GetProperty("LastName").GetString() ?? "Gost",
                    Email = data.GetProperty("Email").GetString() ?? "email@example.com",
                    PhoneNumber = data.GetProperty("PhoneNumber").GetString() ?? "000000000",
                    DateOfBirth = DateTime.TryParse(data.GetProperty("DateOfBirth").GetString(), out var dob) ? dob : new DateTime(1990, 1, 1),
                    DocumentNumber = data.GetProperty("DocumentNumber").GetString() ?? "HR00000"
                };

                _dbContext.Guests.Add(guest);
                await _dbContext.SaveChangesAsync();
                return Ok(new { success = true, action = "Kreiran gost", details = $"{guest.FirstName} {guest.LastName} ({guest.Email})", rawJson = resultJson });
            }
            else if (action == "CreateHotel")
            {
                var hotel = new Hotel
                {
                    Name = data.GetProperty("Name").GetString() ?? "Novi Hotel",
                    Address = data.GetProperty("Address").GetString() ?? "Ulica 1",
                    City = data.GetProperty("City").GetString() ?? "Grad",
                    Rating = data.GetProperty("Rating").GetInt32(),
                    PhoneNumber = data.GetProperty("PhoneNumber").GetString() ?? "000000000"
                };

                _dbContext.Hotels.Add(hotel);
                await _dbContext.SaveChangesAsync();
                return Ok(new { success = true, action = "Kreiran hotel", details = $"{hotel.Name}, {hotel.City} (Ocjena: {hotel.Rating})", rawJson = resultJson });
            }
            else if (action == "CreateRoom")
            {
                int hotelId = data.GetProperty("HotelId").GetInt32();
                var hotelExists = await _dbContext.Hotels.AnyAsync(h => h.Id == hotelId);
                if (!hotelExists)
                {
                    // If hotelId does not exist, use the first hotel or return error
                    var firstHotel = await _dbContext.Hotels.FirstOrDefaultAsync();
                    if (firstHotel == null)
                    {
                        return BadRequest(new { success = false, message = "Nema hotela u bazi za dodavanje sobe." });
                    }
                    hotelId = firstHotel.Id;
                }

                string roomTypeStr = data.GetProperty("RoomType").GetString() ?? "Double";
                RoomType roomType = RoomType.Double;
                if (Enum.TryParse<RoomType>(roomTypeStr, true, out var parsedType))
                {
                    roomType = parsedType;
                }

                var room = new Room
                {
                    HotelId = hotelId,
                    RoomNumber = data.GetProperty("RoomNumber").GetString() ?? "100",
                    Floor = data.GetProperty("Floor").GetInt32(),
                    Capacity = data.GetProperty("Capacity").GetInt32(),
                    PricePerNight = data.GetProperty("PricePerNight").GetDecimal(),
                    RoomType = roomType,
                    IsAvailable = data.GetProperty("IsAvailable").GetBoolean()
                };

                _dbContext.Rooms.Add(room);
                await _dbContext.SaveChangesAsync();
                return Ok(new { success = true, action = "Kreirana soba", details = $"Soba {room.RoomNumber} u hotelu (ID: {room.HotelId}), Cijena: {room.PricePerNight} EUR", rawJson = resultJson });
            }

            return BadRequest(new { success = false, message = "Prepoznata akcija nije podržana.", rawJson = resultJson });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Greška kod obrade JSON-a: {ex.Message}", rawJson = resultJson });
        }
    }

    private async Task<string> CallGeminiApiAsync(string query, string apiKey)
    {
        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        var systemInstruction = @"You are a structured parser for a Hotel Management Database. Parse the user's natural language request into a valid JSON object.
Output only valid, raw JSON (do not wrap in markdown ```json blocks).

The JSON object must have 'action' (string) and 'data' (object) properties. Supported actions:
1. CreateGuest:
   data properties: FirstName (string), LastName (string), Email (string), PhoneNumber (string), DateOfBirth (string 'YYYY-MM-DD'), DocumentNumber (string)
2. CreateHotel:
   data properties: Name (string), Address (string), City (string), Rating (integer 1-5), PhoneNumber (string)
3. CreateRoom:
   data properties: HotelId (integer, default to 1), RoomNumber (string), Floor (integer), Capacity (integer), PricePerNight (decimal), RoomType (string 'Single'|'Double'|'Suite'), IsAvailable (boolean, default to true)

Example Input: 'Create a new guest named John Doe born on 1990-05-15, email john@test.com, phone 098765432, passport HR12345'
Example Output: {'action': 'CreateGuest', 'data': {'FirstName': 'John', 'LastName': 'Doe', 'Email': 'john@test.com', 'PhoneNumber': '098765432', 'DateOfBirth': '1990-05-15', 'DocumentNumber': 'HR12345'}}";

        var payload = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = $"Query: {query}\n\nSystem Instruction: {systemInstruction}" } } }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Gemini API error: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var responseDoc = JsonDocument.Parse(responseString);
        var text = responseDoc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text ?? string.Empty;
    }

    private string ParseUsingFallbackRules(string query)
    {
        query = query.ToLower();

        // Fallback for Guest Creation
        if (query.Contains("gost") || query.Contains("guest") || query.Contains("gosta"))
        {
            var firstName = "Ivan";
            var lastName = "Horvat";
            var email = "ivan.horvat@test.com";
            var phone = "091234567";
            var document = "HR99999";
            var dob = "1990-05-05";

            // Extract email
            var emailMatch = Regex.Match(query, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            if (emailMatch.Success) email = emailMatch.Value;

            // Extract phone number (sequence of 6+ digits)
            var phoneMatch = Regex.Match(query, @"\d{6,15}");
            if (phoneMatch.Success) phone = phoneMatch.Value;

            // Extract name (simple parsing of Capitalized words or following "ime" / "gost" / "gosta" / "goste")
            var nameMatch = Regex.Match(query, @"(?:ime|imenom|gost|gosta|goste|naziv):?\s+([a-zčćžšđ]+)\s+([a-zčćžšđ]+)");
            if (nameMatch.Success)
            {
                var f = nameMatch.Groups[1].Value;
                var l = nameMatch.Groups[2].Value;
                firstName = char.ToUpper(f[0]) + f.Substring(1);
                lastName = char.ToUpper(l[0]) + l.Substring(1);
            }

            // Extract DOB (Date of birth) - formats like DD.MM.YYYY. or DD.MM.YYYY or YYYY-MM-DD
            var dobMatch = Regex.Match(query, @"(?:rođen|roden|rođenja|rodenja|dob|born|birth)?:?\s*(\d{1,2})\.(\d{1,2})\.(\d{4})");
            if (dobMatch.Success)
            {
                var day = int.Parse(dobMatch.Groups[1].Value);
                var month = int.Parse(dobMatch.Groups[2].Value);
                var year = int.Parse(dobMatch.Groups[3].Value);
                dob = $"{year:D4}-{month:D2}-{day:D2}";
            }
            else
            {
                var dobMatchIso = Regex.Match(query, @"(\d{4})-(\d{2})-(\d{2})");
                if (dobMatchIso.Success)
                {
                    dob = dobMatchIso.Value;
                }
            }

            // Extract document number
            var docMatch = Regex.Match(query, @"(?:dokument|putovnic[a-z]|osobn[a-z]|passport|card|broj|iskaznic[a-z]):?\s*([a-z0-9]+)");
            if (docMatch.Success)
            {
                document = docMatch.Groups[1].Value.ToUpper();
            }

            return $@"{{
                ""action"": ""CreateGuest"",
                ""data"": {{
                    ""FirstName"": ""{firstName}"",
                    ""LastName"": ""{lastName}"",
                    ""Email"": ""{email}"",
                    ""PhoneNumber"": ""{phone}"",
                    ""DateOfBirth"": ""{dob}"",
                    ""DocumentNumber"": ""{document}""
                }}
            }}";
        }

        // Fallback for Hotel Creation
        if (query.Contains("hotel"))
        {
            var name = "Hotel Grand";
            var city = "Zagreb";
            var address = "Glavna Ulica 10";
            var rating = 4;
            var phone = "012345678";

            var ratingMatch = Regex.Match(query, @"(?:ocjen[a-z]+|rating|zvjezdic[a-z]+):?\s*(\d)");
            if (ratingMatch.Success && int.TryParse(ratingMatch.Groups[1].Value, out var r))
            {
                rating = Math.Max(1, Math.Min(5, r));
            }

            var cityMatch = Regex.Match(query, @"(?:u|gradu|grad):?\s+([a-zčćžšđ]+)");
            if (cityMatch.Success)
            {
                var c = cityMatch.Groups[1].Value;
                city = char.ToUpper(c[0]) + c.Substring(1);
            }

            var nameMatch = Regex.Match(query, @"(?:hotel|nazivom|naziv):?\s+([a-zčćžšđ]+(?:\s+[a-zčćžšđ]+)?)");
            if (nameMatch.Success)
            {
                var n = nameMatch.Groups[1].Value;
                name = string.Join(" ", n.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => char.ToUpper(w[0]) + w.Substring(1)));
            }

            var phoneMatch = Regex.Match(query, @"(?:telefon|mobitel|broj|phone):?\s*(\d{6,15})");
            if (phoneMatch.Success)
            {
                phone = phoneMatch.Groups[1].Value;
            }

            return $@"{{
                ""action"": ""CreateHotel"",
                ""data"": {{
                    ""Name"": ""{name}"",
                    ""Address"": ""{address}"",
                    ""City"": ""{city}"",
                    ""Rating"": {rating},
                    ""PhoneNumber"": ""{phone}""
                }}
            }}";
        }

        // Fallback for Room Creation
        if (query.Contains("soba") || query.Contains("sobu") || query.Contains("room"))
        {
            var roomNumber = "101";
            var floor = 1;
            var capacity = 2;
            var price = 100.00m;
            var type = "Double";

            var numberMatch = Regex.Match(query, @"(?:soba|sobu|broj|room):?\s*(\d+)");
            if (numberMatch.Success) roomNumber = numberMatch.Groups[1].Value;

            var priceMatch = Regex.Match(query, @"(?:cijen[a-z]+|price|cijena):?\s*(\d+(?:\.\d+)?)");
            if (priceMatch.Success && decimal.TryParse(priceMatch.Groups[1].Value, out var p))
            {
                price = p;
            }

            var floorMatch = Regex.Match(query, @"(?:kat|katu|floor):?\s*(\d+)");
            if (floorMatch.Success && int.TryParse(floorMatch.Groups[1].Value, out var f))
            {
                floor = f;
            }

            var capacityMatch = Regex.Match(query, @"(?:kapacitet|kapaciteta|osoba|people|capacity):?\s*(\d+)");
            if (capacityMatch.Success && int.TryParse(capacityMatch.Groups[1].Value, out var cap))
            {
                capacity = cap;
            }

            if (query.Contains("jednokrevetna") || query.Contains("single"))
            {
                type = "Single";
                capacity = 1;
            }
            else if (query.Contains("apartman") || query.Contains("suite"))
            {
                type = "Suite";
                capacity = 4;
            }

            return $@"{{
                ""action"": ""CreateRoom"",
                ""data"": {{
                    ""HotelId"": 1,
                    ""RoomNumber"": ""{roomNumber}"",
                    ""Floor"": {floor},
                    ""Capacity"": {capacity},
                    ""PricePerNight"": {price},
                    ""RoomType"": ""{type}"",
                    ""IsAvailable"": true
                }}
            }}";
        }

        // Return empty action if nothing matches
        return "{}";
    }
}
