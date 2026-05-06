# Entity Framework Skill

Use this skill when modifying or extending the EF data layer for the Lab2 application.

## Purpose
- Add or update EF entity classes and relationships.
- Configure `DbContext` and database connection strings.
- Implement repository patterns against EF.
- Generate and apply migrations.

## Recommended Workflow
1. Update model entity classes in `Lab2.Model\Entities`.
2. Update `Lab2.Model\HotelDbContext.cs` to include new `DbSet<T>` and relationships.
3. Update `Lab2.Web\Program.cs` to register `HotelDbContext` and the EF repository.
4. Run migrations using the commands in `migration-commands.md`.
