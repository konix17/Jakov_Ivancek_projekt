# EF Migration Commands

Use these commands for the current Hotel-Mgt project structure. Run them from the solution root when `dotnet ef` is installed.

## Add a new migration

```bash
cd /Users/jakovivancek/Desktop/Jakov_Ivancek_projekt
dotnet ef migrations add <MigrationName> --project Hotel-Mgt.Model --startup-project Hotel-Mgt.Web --context HotelDbContext
```

## Apply migrations to the database

```bash
dotnet ef database update --project Hotel-Mgt.Model --startup-project Hotel-Mgt.Web --context HotelDbContext
```

## Remove the last migration

```bash
dotnet ef migrations remove --project Hotel-Mgt.Model --startup-project Hotel-Mgt.Web --context HotelDbContext
```

## Changing database provider

If you switch providers (for example from SQL Server to SQLite), remove old provider-specific migrations and recreate them in the current provider:

```bash
rm -rf Hotel-Mgt.Model/Migrations
dotnet ef migrations add Initial --project Hotel-Mgt.Model --startup-project Hotel-Mgt.Web --context HotelDbContext
```

Then update the database again.

## Configuration notes

- Keep `Hotel-Mgt.Model` as the migrations project.
- Keep `Hotel-Mgt.Web` as the startup project.
- Update `Hotel-Mgt.Web/appsettings.json` or `Hotel-Mgt.Web/appsettings.Development.json` with the correct connection string.
- If you use SQLite locally, a local database file like `Hotel-Mgt.Web/HotelDb.db` will be created.
