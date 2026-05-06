# Lab3 Report and Test Preparation

## 1. Overview
This document explains what was done in the project to complete Lab3: Entity Framework setup, custom routing, database seeding, documentation, and skill usage. It also includes the exact concepts you can explain to your professor.

The application is an ASP.NET Core MVC project with three layers:
- `Lab2.Web` (now referenced as the web layer for MVC controllers and views)
- `Lab2.Model` (now referenced as the model layer for EF and entities)
- `Lab2.Console` (console test/demo project)

> Note: actual physical folder rename from `Lab2.*` to `Lab3.*` was intended but the current workspace still retains the original folder names. The code namespaces and project references have been updated to use `Lab3.*`.

## 2. EF Configuration and Model Adaptation

### 2.1 What was changed
- Installed Entity Framework packages in the model and web projects:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Design`
- Created `HotelDbContext` inheriting from `DbContext`.
- Added `DbSet<T>` properties for each entity used in the database.
- Configured relationships and mappings inside `OnModelCreating`.

### 2.2 Model relationships and EF readiness
- The entity models use EF conventions for primary keys: `Id` on each entity.
- Relationships were configured with EF fluent API:
  - `Hotel` has many `Room`s, `Employee`s, `Review`s, and `Service`s.
  - `Room` belongs to a `Hotel` and has many `Reservation`s.
  - `Reservation` belongs to a `Guest`, a `Room`, and can have many `Service`s.
  - `Payment` belongs to a `Reservation`.
- Set decimal precision for price-related fields using `HasPrecision(18, 2)`.
- Added a many-to-many relationship between `Reservation` and `Service` using `HasMany().WithMany()`.
- Handled delete behavior to avoid cascade cycles where needed.

### 2.3 Why this matters for EF
- EF needs navigation properties and collections to understand relationships.
- Using `DbSet<T>` gives EF a mapping to each table.
- Fluent API ensures the model works correctly when generating the database schema.

## 3. Dependency Injection and Repository Pattern

### 3.1 What was done
- Registered `HotelDbContext` in `Program.cs` with SQL Server:
  - `builder.Services.AddDbContext<HotelDbContext>(options => options.UseSqlServer(...))`
- Added a repository interface `IHotelRepository` and an implementation `EfHotelRepository`.
- Registered the repository as scoped service:
  - `builder.Services.AddScoped<IHotelRepository, EfHotelRepository>();`

### 3.2 Why this is important
- Controllers receive `IHotelRepository` via constructor injection.
- This keeps controllers thin and testable.
- The repository handles EF queries and returns data to the MVC layer.

### 3.3 Example
`HotelsController.Index` calls `_repository.GetAllHotels()` and returns the view with the list.

## 4. Database Migrations and Seeding

### 4.1 Migration setup
- Used EF migrations to create and update the database schema.
- The correct command pattern for multi-project setup was:
  - `dotnet ef migrations add InitialWithData --project Lab2.Web --context HotelDbContext`
  - `dotnet ef database update --project Lab2.Web --context HotelDbContext`
- The startup project has the connection string; the migrations assembly is the model project.

### 4.2 Seeding data
Added `modelBuilder.Entity<...>().HasData(...)` in `HotelDbContext.OnModelCreating` for:
- Hotels
- Rooms
- Services
- Employees
- Guests
- Reservations
- Payments
- Reviews

### 4.3 Why seeding is useful
- Provides initial data for testing.
- Makes the app functional immediately after database creation.
- Demonstrates how EF can initialize lookup data and test entities.

## 5. Custom Routing

### 5.1 What was done
- Added attribute routes to controllers, e.g. `HotelsController` using `[Route("hoteli")]`.
- Added custom route templates for actions like Details: `[Route("{id:int}")]`.
- Used Croatian-friendly URLs instead of default `/Controller/Action`.

### 5.2 Example routes
- `/hoteli` → `HotelsController.Index`
- `/hoteli/1` → `HotelsController.Details(int id)`
- Similar custom routing was used for other controllers.

### 5.3 Why this matters
- Shows understanding of attribute routing versus conventional routing.
- Demonstrates how to make URLs more user-friendly and localized.

## 6. Documentation and Lab3 Requirements

### 6.1 Semantic model file
- `semantic-model.md` should describe:
  - Each table/entity
  - Key fields
  - Primary relationships between entities
- It documents the domain model in human-readable form.

### 6.2 Sitemap file
- `sitemap.md` should list all application routes and their controller/action/view mappings.
- It proves you understand how URLs map into MVC actions.

### 6.3 Skills
- The lab asked to configure skill files for AI-assisted coding.
- The agent handled EF and routing backend work, and was ready to delegate UI tasks if needed.
- The actual `SKILL.md` file is used to define those skill workflows.

## 7. What to explain in the test

### 7.1 Entity Framework concepts
Explain these clearly:
- `DbContext` is the EF gateway to the database.
- `DbSet<T>` represents a table.
- `OnModelCreating` configures relationships and schema details.
- Migrations synchronize C# model changes with the database schema.
- Seeding inserts initial data through `HasData()`.

### 7.2 Routing concepts
Explain these clearly:
- Default route uses `{controller=Home}/{action=Index}/{id?}`.
- Attribute routing overrides URL mapping with `[Route(...)]`.
- Route constraints like `{id:int}` ensure the parameter is numeric.
- Custom routing can create friendly URLs such as `/hoteli`.

### 7.3 Dependency injection and repositories
Explain these clearly:
- Controller receives services by constructor injection.
- Using `IHotelRepository` decouples controllers from EF implementation.
- `AddScoped` ensures one repository instance per request.

### 7.4 Practical flow in this app
- Request arrives in the browser.
- Route engine matches the URL to the correct controller/action.
- Controller calls the repository.
- Repository uses `HotelDbContext` and EF to query the database.
- The action returns a view with the model data.

## 8. Common professor questions and answers

### Q: Why did you choose EF instead of mock data?
A: EF provides a real relational database backend, supports persistence, migrations, and can scale beyond in-memory mock repositories.

### Q: How do migrations work?
A: Migrations compare the model code to the current schema and generate SQL changes. `dotnet ef migrations add` creates the migration file; `dotnet ef database update` applies it.

### Q: What is `HasData()` used for?
A: It seeds initial data during migration so the database contains useful records right after creation.

### Q: Why use attribute routing?
A: Attribute routing gives fine-grained control over URLs and lets you use custom paths like `/hoteli` instead of default controller/action patterns.

### Q: How did you handle SQL Server connection issues?
A: Use `localhost\SQLEXPRESS` or `.`\SQLEXPRESS and `Windows Authentication`, and if SSMS shows certificate warnings, enable `Trust server certificate` in the connect properties.

## 9. Practical steps to run the app
1. Open the root folder in VS Code.
2. Make sure the connection string in `appsettings.json` points to the local SQL Express instance.
3. Run `dotnet run --project Lab2.Web` from the project root.
4. Visit `/hoteli` in the browser to see seeded hotel data.

## 10. Notes on the current workspace
- The source folder names are still `Lab2.Console`, `Lab2.Model`, `Lab2.Web`.
- The code namespaces and references were updated to `Lab3.*`.
- If the professor asks, explain that the app uses Lab3 logical naming in code but the workspace file names were not changed by the editor tool.

---

### Final advice for the exam
- Speak in simple steps: `model`, `DbContext`, `repository`, `routing`, `views`.
- Use the actual app example: `HotelsController.Index loads hotels from EF via repository.`
- Mention the database seed data: `HasData created hotels, rooms, services, guests, reservations, payments, and reviews.`
- Show that you understand both `EF` and `routing` as separate concepts.
