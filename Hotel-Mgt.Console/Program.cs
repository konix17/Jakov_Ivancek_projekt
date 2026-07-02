using HotelMgt.Model;
using HotelMgt.Model.Entities;
using HotelMgt.Model.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Contains("--mcp"))
        {
            await RunMcpServerAsync();
        }
        else
        {
            RunOriginalDemo();
        }
    }

    static async Task RunMcpServerAsync()
    {
        Console.Error.WriteLine("Starting MCP Server...");
        
        var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Hotel-Mgt.Web", "HotelDb.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        
        using var db = new HotelDbContext(optionsBuilder.Options);
        
        while (true)
        {
            var line = await Console.In.ReadLineAsync();
            if (line == null) break;

            try
            {
                var request = JsonNode.Parse(line);
                if (request == null) continue;

                var id = request["id"]?.GetValue<int>();
                var method = request["method"]?.GetValue<string>();

                if (method == "initialize")
                {
                    var response = new
                    {
                        jsonrpc = "2.0",
                        id = id,
                        result = new
                        {
                            protocolVersion = "2024-11-05",
                            capabilities = new { tools = new { } },
                            serverInfo = new { name = "hotel-mgt-mcp", version = "1.0.0" }
                        }
                    };
                    Console.WriteLine(JsonSerializer.Serialize(response));
                }
                else if (method == "notifications/initialized")
                {
                    // No response required
                }
                else if (method == "tools/list")
                {
                    var response = new
                    {
                        jsonrpc = "2.0",
                        id = id,
                        result = new
                        {
                            tools = new object[]
                            {
                                new { name = "list_hotels", description = "List all hotels", inputSchema = new { type = "object", properties = new { } } },
                                new { name = "list_rooms", description = "List all rooms", inputSchema = new { type = "object", properties = new { } } },
                                new { name = "list_guests", description = "List all guests", inputSchema = new { type = "object", properties = new { } } },
                                new { name = "search_guests", description = "Search guests", inputSchema = new { type = "object", properties = new { query = new { type = "string" } }, required = new[] { "query" } } },
                                new { name = "add_hotel", description = "Add a new hotel", inputSchema = new { type = "object", properties = new { name = new { type = "string" }, city = new { type = "string" }, address = new { type = "string" }, rating = new { type = "integer" } }, required = new[] { "name", "city", "address" } } }
                            }
                        }
                    };
                    Console.WriteLine(JsonSerializer.Serialize(response));
                }
                else if (method == "tools/call")
                {
                    var toolName = request["params"]?["name"]?.GetValue<string>();
                    var arguments = request["params"]?["arguments"];

                    object result = "Tool not found";
                    
                    if (toolName == "list_hotels")
                    {
                        var hotels = await db.Hotels.ToListAsync();
                        result = hotels.Select(h => new { h.Id, h.Name, h.City, h.Rating });
                    }
                    else if (toolName == "list_rooms")
                    {
                        var rooms = await db.Rooms.Include(r => r.Hotel).ToListAsync();
                        result = rooms.Select(r => new { r.Id, r.RoomNumber, r.PricePerNight, Hotel = r.Hotel.Name });
                    }
                    else if (toolName == "list_guests")
                    {
                        var guests = await db.Guests.ToListAsync();
                        result = guests.Select(g => new { g.Id, g.FirstName, g.LastName, g.Email });
                    }
                    else if (toolName == "search_guests")
                    {
                        var q = arguments?["query"]?.GetValue<string>()?.ToLower() ?? "";
                        var guests = await db.Guests
                            .Where(g => g.FirstName.ToLower().Contains(q) || g.LastName.ToLower().Contains(q) || g.Email.ToLower().Contains(q))
                            .ToListAsync();
                        result = guests.Select(g => new { g.Id, g.FirstName, g.LastName, g.Email });
                    }
                    else if (toolName == "add_hotel")
                    {
                        var name = arguments?["name"]?.GetValue<string>() ?? "";
                        var city = arguments?["city"]?.GetValue<string>() ?? "";
                        var address = arguments?["address"]?.GetValue<string>() ?? "";
                        var rating = arguments?["rating"]?.GetValue<int>() ?? 5;

                        var hotel = new Hotel { Name = name, City = city, Address = address, Rating = rating, PhoneNumber = "0000" };
                        db.Hotels.Add(hotel);
                        await db.SaveChangesAsync();

                        result = new { success = true, hotelId = hotel.Id, message = $"Hotel {name} uspješno dodan." };
                    }

                    var response = new
                    {
                        jsonrpc = "2.0",
                        id = id,
                        result = new
                        {
                            content = new[]
                            {
                                new { type = "text", text = JsonSerializer.Serialize(result) }
                            }
                        }
                    };
                    Console.WriteLine(JsonSerializer.Serialize(response));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error handling line: {ex.Message}");
            }
        }
    }

    static void RunOriginalDemo()
    {
        List<Hotel> hotels = new List<Hotel>();

        Hotel hotel1 = new Hotel
        {
            Id = 1,
            Name = "Adriatic Hotel",
            Address = "Ilica 10",
            City = "Zagreb",
            Rating = 4,
            PhoneNumber = "0911111111"
        };

        Hotel hotel2 = new Hotel
        {
            Id = 2,
            Name = "Sunset Resort",
            Address = "Riva 20",
            City = "Split",
            Rating = 5,
            PhoneNumber = "0922222222"
        };

        Hotel hotel3 = new Hotel
        {
            Id = 3,
            Name = "Mountain View Hotel",
            Address = "Korzo 5",
            City = "Rijeka",
            Rating = 3,
            PhoneNumber = "0933333333"
        };

        Room room1 = new Room
        {
            Id = 1,
            RoomNumber = "101",
            Floor = 1,
            Capacity = 1,
            PricePerNight = 80,
            RoomType = RoomType.Single,
            IsAvailable = true,
            Hotel = hotel1
        };

        Room room2 = new Room
        {
            Id = 2,
            RoomNumber = "102",
            Floor = 1,
            Capacity = 2,
            PricePerNight = 120,
            RoomType = RoomType.Double,
            IsAvailable = true,
            Hotel = hotel1
        };

        Room room3 = new Room
        {
            Id = 3,
            RoomNumber = "201",
            Floor = 2,
            Capacity = 3,
            PricePerNight = 250,
            RoomType = RoomType.Suite,
            IsAvailable = false,
            Hotel = hotel2
        };

        Room room4 = new Room
        {
            Id = 4,
            RoomNumber = "202",
            Floor = 2,
            Capacity = 2,
            PricePerNight = 150,
            RoomType = RoomType.Double,
            IsAvailable = true,
            Hotel = hotel2
        };

        Room room5 = new Room
        {
            Id = 5,
            RoomNumber = "301",
            Floor = 3,
            Capacity = 1,
            PricePerNight = 70,
            RoomType = RoomType.Single,
            IsAvailable = true,
            Hotel = hotel3
        };

        Room room6 = new Room
        {
            Id = 6,
            RoomNumber = "302",
            Floor = 3,
            Capacity = 3,
            PricePerNight = 180,
            RoomType = RoomType.Suite,
            IsAvailable = false,
            Hotel = hotel3
        };

        hotel1.Rooms.Add(room1);
        hotel1.Rooms.Add(room2);
        hotel2.Rooms.Add(room3);
        hotel2.Rooms.Add(room4);
        hotel3.Rooms.Add(room5);
        hotel3.Rooms.Add(room6);

        Service service1 = new Service
        {
            Id = 1,
            Name = "Breakfast",
            Description = "Buffet breakfast",
            Price = 15,
            IsAvailable = true,
            Hotel = hotel1
        };

        Service service2 = new Service
        {
            Id = 2,
            Name = "Parking",
            Description = "Private hotel parking",
            Price = 10,
            IsAvailable = true,
            Hotel = hotel1
        };

        Service service3 = new Service
        {
            Id = 3,
            Name = "Spa",
            Description = "Spa and wellness access",
            Price = 30,
            IsAvailable = true,
            Hotel = hotel2
        };

        Service service4 = new Service
        {
            Id = 4,
            Name = "Dinner",
            Description = "Hotel restaurant dinner",
            Price = 25,
            IsAvailable = true,
            Hotel = hotel3
        };

        hotel1.Services.Add(service1);
        hotel1.Services.Add(service2);
        hotel2.Services.Add(service3);
        hotel3.Services.Add(service4);

        Employee employee1 = new Employee
        {
            Id = 1,
            FirstName = "Marko",
            LastName = "Maric",
            Email = "marko@adriatic.hr",
            PhoneNumber = "0991111111",
            Salary = 1200,
            Role = EmployeeRole.Manager,
            HireDate = new DateTime(2022, 5, 10),
            Hotel = hotel1
        };

        Employee employee2 = new Employee
        {
            Id = 2,
            FirstName = "Ivana",
            LastName = "Ivic",
            Email = "ivana@sunset.hr",
            PhoneNumber = "0992222222",
            Salary = 1000,
            Role = EmployeeRole.Receptionist,
            HireDate = new DateTime(2023, 2, 15),
            Hotel = hotel2
        };

        Employee employee3 = new Employee
        {
            Id = 3,
            FirstName = "Petra",
            LastName = "Peric",
            Email = "petra@mountain.hr",
            PhoneNumber = "0993333333",
            Salary = 900,
            Role = EmployeeRole.Housekeeping,
            HireDate = new DateTime(2024, 1, 20),
            Hotel = hotel3
        };

        hotel1.Employees.Add(employee1);
        hotel2.Employees.Add(employee2);
        hotel3.Employees.Add(employee3);

        Guest guest1 = new Guest
        {
            Id = 1,
            FirstName = "Ivan",
            LastName = "Ivic",
            Email = "ivan@gmail.com",
            PhoneNumber = "0951111111",
            DateOfBirth = new DateTime(1999, 5, 10),
            DocumentNumber = "HR12345"
        };

        Guest guest2 = new Guest
        {
            Id = 2,
            FirstName = "Ana",
            LastName = "Anic",
            Email = "ana@gmail.com",
            PhoneNumber = "0952222222",
            DateOfBirth = new DateTime(2000, 8, 15),
            DocumentNumber = "HR67890"
        };

        Guest guest3 = new Guest
        {
            Id = 3,
            FirstName = "Luka",
            LastName = "Lukic",
            Email = "luka@gmail.com",
            PhoneNumber = "0953333333",
            DateOfBirth = new DateTime(1998, 3, 22),
            DocumentNumber = "HR54321"
        };

        Reservation reservation1 = new Reservation
        {
            Id = 1,
            ReservationCode = "RES001",
            ReservationDate = new DateTime(2026, 3, 20),
            CheckInDate = new DateTime(2026, 4, 10),
            CheckOutDate = new DateTime(2026, 4, 15),
            TotalPrice = 400,
            Status = ReservationStatus.Confirmed,
            Guest = guest1,
            Room = room1
        };

        Reservation reservation2 = new Reservation
        {
            Id = 2,
            ReservationCode = "RES002",
            ReservationDate = new DateTime(2026, 3, 22),
            CheckInDate = new DateTime(2026, 5, 1),
            CheckOutDate = new DateTime(2026, 5, 3),
            TotalPrice = 240,
            Status = ReservationStatus.Pending,
            Guest = guest2,
            Room = room2
        };

        Reservation reservation3 = new Reservation
        {
            Id = 3,
            ReservationCode = "RES003",
            ReservationDate = new DateTime(2026, 3, 25),
            CheckInDate = new DateTime(2026, 6, 5),
            CheckOutDate = new DateTime(2026, 6, 10),
            TotalPrice = 900,
            Status = ReservationStatus.Confirmed,
            Guest = guest3,
            Room = room3
        };

        Reservation reservation4 = new Reservation
        {
            Id = 4,
            ReservationCode = "RES004",
            ReservationDate = new DateTime(2026, 3, 27),
            CheckInDate = new DateTime(2026, 7, 1),
            CheckOutDate = new DateTime(2026, 7, 4),
            TotalPrice = 540,
            Status = ReservationStatus.Cancelled,
            Guest = guest1,
            Room = room6
        };

        Payment payment1 = new Payment
        {
            Id = 1,
            Amount = 400,
            PaymentDate = new DateTime(2026, 3, 21),
            PaymentMethod = PaymentMethod.Card,
            IsPaid = true,
            Reservation = reservation1
        };

        Payment payment2 = new Payment
        {
            Id = 2,
            Amount = 120,
            PaymentDate = new DateTime(2026, 3, 23),
            PaymentMethod = PaymentMethod.Cash,
            IsPaid = false,
            Reservation = reservation2
        };

        Payment payment3 = new Payment
        {
            Id = 3,
            Amount = 900,
            PaymentDate = new DateTime(2026, 3, 26),
            PaymentMethod = PaymentMethod.Card,
            IsPaid = true,
            Reservation = reservation3
        };

        Payment payment4 = new Payment
        {
            Id = 4,
            Amount = 540,
            PaymentDate = new DateTime(2026, 3, 28),
            PaymentMethod = PaymentMethod.Cash,
            IsPaid = false,
            Reservation = reservation4
        };

        reservation1.Payments.Add(payment1);
        reservation2.Payments.Add(payment2);
        reservation3.Payments.Add(payment3);
        reservation4.Payments.Add(payment4);

        reservation1.Services.Add(service1);
        reservation1.Services.Add(service2);
        reservation2.Services.Add(service1);
        reservation3.Services.Add(service3);
        reservation4.Services.Add(service4);

        service1.Reservations.Add(reservation1);
        service1.Reservations.Add(reservation2);
        service2.Reservations.Add(reservation1);
        service3.Reservations.Add(reservation3);
        service4.Reservations.Add(reservation4);

        guest1.Reservations.Add(reservation1);
        guest1.Reservations.Add(reservation4);
        guest2.Reservations.Add(reservation2);
        guest3.Reservations.Add(reservation3);

        room1.Reservations.Add(reservation1);
        room2.Reservations.Add(reservation2);
        room3.Reservations.Add(reservation3);
        room6.Reservations.Add(reservation4);

        hotel1.Reservations.Add(reservation1);
        hotel1.Reservations.Add(reservation2);
        hotel2.Reservations.Add(reservation3);
        hotel3.Reservations.Add(reservation4);

        Review review1 = new Review
        {
            Id = 1,
            Rating = 5,
            Comment = "Excellent service and clean rooms.",
            CreatedAt = new DateTime(2026, 3, 30),
            Guest = guest1,
            Hotel = hotel1
        };

        Review review2 = new Review
        {
            Id = 2,
            Rating = 4,
            Comment = "Very nice spa experience.",
            CreatedAt = new DateTime(2026, 3, 29),
            Guest = guest3,
            Hotel = hotel2
        };

        guest1.Reviews.Add(review1);
        guest3.Reviews.Add(review2);

        hotels.Add(hotel1);
        hotels.Add(hotel2);
        hotels.Add(hotel3);

        Console.WriteLine("Hotels loaded successfully.");
        Console.WriteLine($"Number of hotels: {hotels.Count}");
        Console.WriteLine($"Total reservations: {hotels.SelectMany(h => h.Reservations).Count()}");
        Console.WriteLine($"Total guests: {hotels.SelectMany(h => h.Reservations).Select(r => r.Guest).Distinct().Count()}");
        Console.WriteLine();
        Console.WriteLine("===== LINQ UPITI =====");

        var zagrebHotels = hotels
            .Where(h => h.City == "Zagreb")
            .ToList();

        Console.WriteLine("\n1. Hoteli u Zagrebu:");
        foreach (var hotel in zagrebHotels)
        {
            Console.WriteLine($"- {hotel.Name}");
        }

        var confirmedReservations = hotels
            .SelectMany(h => h.Reservations)
            .Where(r => r.Status == ReservationStatus.Confirmed)
            .ToList();

        Console.WriteLine("\n2. Potvrđene rezervacije:");
        foreach (var reservation in confirmedReservations)
        {
            Console.WriteLine($"- {reservation.ReservationCode} | Gost: {reservation.Guest.FirstName} {reservation.Guest.LastName}");
        }

        var expensiveRooms = hotels
            .SelectMany(h => h.Rooms)
            .Where(r => r.PricePerNight > 100)
            .OrderByDescending(r => r.PricePerNight)
            .ToList();

        Console.WriteLine("\n3. Sobe skuplje od 100:");
        foreach (var room in expensiveRooms)
        {
            Console.WriteLine($"- Soba {room.RoomNumber} | {room.PricePerNight} EUR | Hotel: {room.Hotel.Name}");
        }

        List<Guest> allGuests = new List<Guest> { guest1, guest2, guest3 };

        var guestsWithMultipleReservations = allGuests
            .Where(g => g.Reservations.Count > 1)
            .ToList();

        Console.WriteLine("\n4. Gosti s više od jedne rezervacije:");
        foreach (var guest in guestsWithMultipleReservations)
        {
            Console.WriteLine($"- {guest.FirstName} {guest.LastName} | Broj rezervacija: {guest.Reservations.Count}");
        }

        var revenueByHotel = hotels
            .Select(h => new
            {
                HotelName = h.Name,
                Revenue = h.Reservations
                    .Where(r => r.Status == ReservationStatus.Confirmed)
                    .Sum(r => r.TotalPrice)
            })
            .OrderByDescending(x => x.Revenue)
            .ToList();

        Console.WriteLine("\n5. Ukupan prihod po hotelu:");
        foreach (var item in revenueByHotel)
        {
            Console.WriteLine($"- {item.HotelName} | Prihod: {item.Revenue} EUR");
        }

        var usedServices = hotels
            .SelectMany(h => h.Reservations)
            .SelectMany(r => r.Services)
            .Distinct()
            .ToList();

        Console.WriteLine("\n6. Korištene usluge:");
        foreach (var service in usedServices)
        {
            Console.WriteLine($"- {service.Name}");
        }

        var nextReservation = hotels
            .SelectMany(h => h.Reservations)
            .Where(r => r.CheckInDate > DateTime.Now)
            .OrderBy(r => r.CheckInDate)
            .FirstOrDefault();

        Console.WriteLine("\n7. Prva nadolazeća rezervacija:");
        if (nextReservation != null)
        {
            Console.WriteLine($"- {nextReservation.ReservationCode} | {nextReservation.CheckInDate:dd.MM.yyyy.} | Gost: {nextReservation.Guest.FirstName} {nextReservation.Guest.LastName}");
        }
        else
        {
            Console.WriteLine("- Nema nadolazećih rezervacija.");
        }

        var roomCountByHotel = hotels
            .Select(h => new
            {
                HotelName = h.Name,
                RoomCount = h.Rooms.Count
            })
            .ToList();

        Console.WriteLine("\n8. Broj soba po hotelu:");
        foreach (var item in roomCountByHotel)
        {
            Console.WriteLine($"- {item.HotelName} | Broj soba: {item.RoomCount}");
        }

        var unpaidPayments = hotels
            .SelectMany(h => h.Reservations)
            .SelectMany(r => r.Payments)
            .Where(p => !p.IsPaid)
            .ToList();

        Console.WriteLine("\n9. Neplaćena plaćanja:");
        foreach (var payment in unpaidPayments)
        {
            Console.WriteLine($"- Iznos: {payment.Amount} EUR | Metoda: {payment.PaymentMethod} | Rezervacija: {payment.Reservation.ReservationCode}");
        }

        var mostExpensiveRoom = hotels
            .SelectMany(h => h.Rooms)
            .OrderByDescending(r => r.PricePerNight)
            .FirstOrDefault();

        Console.WriteLine("\n10. Najskuplja soba:");
        if (mostExpensiveRoom != null)
        {
            Console.WriteLine($"- Soba {mostExpensiveRoom.RoomNumber} | {mostExpensiveRoom.PricePerNight} EUR | Hotel: {mostExpensiveRoom.Hotel.Name}");
        }
    }
}