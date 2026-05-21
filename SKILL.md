# Entity Framework Skill

This skill is for ongoing development of the Hotel-Mgt application data layer and its integration with web UI. Use it for adding, updating, or refactoring database entities, relationships, context configuration, migrations, and repository logic.

## Scope
- Define and update EF Core entity classes in `Hotel-Mgt.Model/Entities`.
- Configure `HotelDbContext` and relational mappings in `Hotel-Mgt.Model/HotelDbContext.cs`.
- Change and validate database providers and connection strings.
- Implement or revise repository patterns in `Hotel-Mgt.Web/Repositories`.
- Create, update, and apply EF Core migrations.
- Ensure the web project uses the correct startup configuration and migrations assembly.

## Recommended Workflow
1. Change or add entity classes in `Hotel-Mgt.Model/Entities`.
2. Update `Hotel-Mgt.Model/HotelDbContext.cs`:
   - Add new `DbSet<T>` properties.
   - Configure relationships, delete behavior, and precision settings.
   - Seed initial data if needed.
3. Update `Hotel-Mgt.Web/Program.cs`:
   - Register `HotelDbContext` with `builder.Services.AddDbContext<HotelDbContext>(...)`.
   - Choose the correct provider (`UseSqlite`, `UseSqlServer`, etc.).
   - Ensure the startup project matches the migrations project when generating migrations.
4. Update `Hotel-Mgt.Web/appsettings.json` or `appsettings.Development.json` with the provider connection string.
5. Rebuild the solution and verify the project compiles.
6. Manage migrations:
   - Add new migrations with `dotnet ef migrations add <Name> --project Hotel-Mgt.Model --startup-project Hotel-Mgt.Web`
   - Apply database updates with `dotnet ef database update --project Hotel-Mgt.Model --startup-project Hotel-Mgt.Web`
   - If changing providers, remove provider-specific migrations and recreate them.
7. Test the application end-to-end by running `dotnet run` from `Hotel-Mgt.Web` and verifying data access.

## Notes
- Prefer `Hotel-Mgt.Model` as the migrations assembly and `Hotel-Mgt.Web` as the startup project.
- Keep provider-specific packages aligned in both projects when switching databases.
- Use SQLite for local Mac development and a server-based provider for production if needed.
- Warnings about non-nullable properties can be fixed by adding `required` or nullable annotations to entity properties.
