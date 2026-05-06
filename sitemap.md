# Sitemap

## Main routes and views

- `/` -> `HomeController.Index()` -> `Views/Home/Index.cshtml`
- `/Home/Privacy` -> `HomeController.Privacy()` -> `Views/Home/Privacy.cshtml`
- `/Home/Error` -> `HomeController.Error()` -> `Views/Shared/Error.cshtml`

## Hotel pages

- `/hotels` -> `HotelsController.Index()` -> `Views/Hotels/Index.cshtml`
- `/hotels/{id}` -> `HotelsController.Details(int id)` -> `Views/Hotels/Details.cshtml`

## Room pages

- `/rooms` -> `RoomsController.Index()` -> `Views/Rooms/Index.cshtml`
- `/rooms/{id}` -> `RoomsController.Details(int id)` -> `Views/Rooms/Details.cshtml`

## Service pages

- `/services` -> `ServicesController.Index()` -> `Views/Services/Index.cshtml`
- `/services/{id}` -> `ServicesController.Details(int id)` -> `Views/Services/Details.cshtml`

## Employee pages

- `/employees` -> `EmployeesController.Index()` -> `Views/Employees/Index.cshtml`
- `/employees/{id}` -> `EmployeesController.Details(int id)` -> `Views/Employees/Details.cshtml`

## Guest pages

- `/guests` -> `GuestsController.Index()` -> `Views/Guests/Index.cshtml`
- `/guests/{id}` -> `GuestsController.Details(int id)` -> `Views/Guests/Details.cshtml`

## Reservation pages

- `/reservations` -> `ReservationsController.Index()` -> `Views/Reservations/Index.cshtml`
- `/reservations/{id}` -> `ReservationsController.Details(int id)` -> `Views/Reservations/Details.cshtml`

## Payment pages

- `/payments` -> `PaymentsController.Index()` -> `Views/Payments/Index.cshtml`
- `/payments/{id}` -> `PaymentsController.Details(int id)` -> `Views/Payments/Details.cshtml`

## Review pages

- `/reviews` -> `ReviewsController.Index()` -> `Views/Reviews/Index.cshtml`
- `/reviews/{id}` -> `ReviewsController.Details(int id)` -> `Views/Reviews/Details.cshtml`

## Fallback conventional routes

- `/Controller/Action/{id?}` is still available via the default route in `Program.cs`.
