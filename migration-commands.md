# EF Migration Commands

Run these commands after the database is available and `dotnet ef` is installed.

From the solution root:

```powershell
cd Lab2.Model
dotnet ef migrations add Initial --startup-project ../Lab2.Web --context HotelDbContext
```

Then apply the migration:

```powershell
dotnet ef database update --startup-project ../Lab2.Web --context HotelDbContext
```

If you use a different database provider or connection string, update `Lab2.Web\appsettings.json` accordingly.
