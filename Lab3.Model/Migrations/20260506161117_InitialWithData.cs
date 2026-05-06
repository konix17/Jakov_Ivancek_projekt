using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lab3.Model.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "DateOfBirth", "DocumentNumber", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(1999, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "HR12345", "ivan@gmail.com", "Ivan", "Ivic", "0951111111" },
                    { 2, new DateTime(2000, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "HR67890", "ana@gmail.com", "Ana", "Anic", "0952222222" },
                    { 3, new DateTime(1998, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "HR54321", "luka@gmail.com", "Luka", "Lukic", "0953333333" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Address", "City", "Name", "PhoneNumber", "Rating" },
                values: new object[,]
                {
                    { 1, "Ilica 10", "Zagreb", "Adriatic Hotel", "0911111111", 4 },
                    { 2, "Riva 20", "Split", "Sunset Resort", "0922222222", 5 },
                    { 3, "Korzo 5", "Rijeka", "Mountain View Hotel", "0933333333", 3 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "FirstName", "HireDate", "HotelId", "LastName", "PhoneNumber", "Role", "Salary" },
                values: new object[,]
                {
                    { 1, "marko@adriatic.hr", "Marko", new DateTime(2022, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Maric", "0991111111", 1, 1200m },
                    { 2, "ivana@sunset.hr", "Ivana", new DateTime(2023, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Ivic", "0992222222", 2, 1000m },
                    { 3, "petra@mountain.hr", "Petra", new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Peric", "0993333333", 3, 900m }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "GuestId", "HotelId", "Rating" },
                values: new object[,]
                {
                    { 1, "Excellent service and clean rooms.", new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 5 },
                    { 2, "Very nice spa experience.", new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, 4 }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "Floor", "HotelId", "IsAvailable", "PricePerNight", "RoomNumber", "RoomType" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, true, 80m, "101", 1 },
                    { 2, 2, 1, 1, true, 120m, "102", 2 },
                    { 3, 3, 2, 2, false, 250m, "201", 3 },
                    { 4, 2, 2, 2, true, 150m, "202", 2 },
                    { 5, 1, 3, 3, true, 70m, "301", 1 },
                    { 6, 3, 3, 3, false, 180m, "302", 3 }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "HotelId", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Buffet breakfast", 1, true, "Breakfast", 15m },
                    { 2, "Private hotel parking", 1, true, "Parking", 10m },
                    { 3, "Spa and wellness access", 2, true, "Spa", 30m },
                    { 4, "Hotel restaurant dinner", 3, true, "Dinner", 25m }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "CheckInDate", "CheckOutDate", "GuestId", "HotelId", "ReservationCode", "ReservationDate", "RoomId", "Status", "TotalPrice" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, "RES001", new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 400m },
                    { 2, new DateTime(2026, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, null, "RES002", new DateTime(2026, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 600m },
                    { 3, new DateTime(2026, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, null, "RES003", new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, 750m },
                    { 4, new DateTime(2026, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, "RES004", new DateTime(2026, 3, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 3, 540m }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "IsPaid", "PaymentDate", "PaymentMethod", "ReservationId" },
                values: new object[,]
                {
                    { 1, 400m, true, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 2, 600m, true, new DateTime(2026, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 3, 750m, false, new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3 },
                    { 4, 540m, false, new DateTime(2026, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
