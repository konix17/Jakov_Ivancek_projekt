# Sitemap

## Main routes and views

- `/` -> `HomeController.Index()` -> `Views/Home/Index.cshtml`
- `/Home/Privacy` -> `HomeController.Privacy()` -> `Views/Home/Privacy.cshtml`
- `/ai-assistant` -> `HomeController.AiAssistant()` -> `Views/Home/AiAssistant.cshtml` (Admin only)
- `/Home/Error` -> `HomeController.Error()` -> `Views/Shared/Error.cshtml`

## Hotel pages (`/hoteli`)

- `/hoteli` -> `HotelsController.Index()` -> `Views/Hotels/Index.cshtml`
- `/hoteli/{id}` -> `HotelsController.Details(int id)` -> `Views/Hotels/Details.cshtml`
- `/hoteli/create`, `/hoteli/edit/{id}`, `/hoteli/delete/{id}` (Admin only)

## Room pages (`/smjestaji`)

- `/smjestaji` -> `RoomsController.Index()` -> `Views/Rooms/Index.cshtml`
- `/smjestaji/{id}` -> `RoomsController.Details(int id)` -> `Views/Rooms/Details.cshtml`
- `/smjestaji/create`, `/smjestaji/edit/{id}`, `/smjestaji/delete/{id}` (Admin only)

## Service pages (`/usluge`)

- `/usluge` -> `ServicesController.Index()` -> `Views/Services/Index.cshtml`
- `/usluge/{id}` -> `ServicesController.Details(int id)` -> `Views/Services/Details.cshtml`
- `/usluge/create`, `/usluge/edit/{id}`, `/usluge/delete/{id}` (Admin only)

## Employee pages (`/zaposlenici`)

- `/zaposlenici` -> `EmployeesController.Index()` -> `Views/Employees/Index.cshtml`
- `/zaposlenici/{id}` -> `EmployeesController.Details(int id)` -> `Views/Employees/Details.cshtml`
- `/zaposlenici/create`, `/zaposlenici/edit/{id}`, `/zaposlenici/delete/{id}` (Admin only)

## Guest pages (`/gosti`)

- `/gosti` -> `GuestsController.Index()` -> `Views/Guests/Index.cshtml`
- `/gosti/{id}` -> `GuestsController.Details(int id)` -> `Views/Guests/Details.cshtml`
- `/gosti/create`, `/gosti/edit/{id}`, `/gosti/delete/{id}` (Admin only)

## Reservation pages (`/rezervacije`)

- `/rezervacije` -> `ReservationsController.Index()` -> `Views/Reservations/Index.cshtml`
- `/rezervacije/{id}` -> `ReservationsController.Details(int id)` -> `Views/Reservations/Details.cshtml`
- `/rezervacije/create`, `/rezervacije/edit/{id}`, `/rezervacije/delete/{id}` (Admin only)
- `/rezervacije/hotel-rooms?hotelId=` -> `ReservationsController.HotelRooms(int hotelId)` (JSON, feeds the room dropdown)

## Payment pages (`/placanja`)

- `/placanja` -> `PaymentsController.Index()` -> `Views/Payments/Index.cshtml`
- `/placanja/{id}` -> `PaymentsController.Details(int id)` -> `Views/Payments/Details.cshtml`
- `/placanja/create`, `/placanja/edit/{id}`, `/placanja/delete/{id}` (Admin only)

## Review pages (`/recenzije`)

- `/recenzije` -> `ReviewsController.Index()` -> `Views/Reviews/Index.cshtml`
- `/recenzije/{id}` -> `ReviewsController.Details(int id)` -> `Views/Reviews/Details.cshtml`
- `/recenzije/create`, `/recenzije/edit/{id}`, `/recenzije/delete/{id}` (Admin only)

## Account & Setup

- `/Account/Login`, `/Account/Register`, `/Account/Logout`, `/Account/ExternalLogin` -> `AccountController`
- `/Setup` -> `SetupController.Index()` — role management (Admin only)
- `/Setup/Logs` -> `SetupController.Logs()` — last 100 requests from `LoggingMiddleware` (Admin only)

## REST API (`/api/...`)

Every resource above also has a matching `[ApiController]` under `Controllers/Api/` (`/api/hotels`, `/api/rooms`, `/api/services`, `/api/employees`, `/api/guests`, `/api/reservations`, `/api/payments`, `/api/reviews`), plus:

- `/api/search?q=` -> `SearchApiController` — global search across pages and records
- `/api/ai/parse` -> `AiApiController` — natural-language data entry (Admin only)
- `/api/hotels/{hotelId}/attachments` -> `AttachmentsApiController` — file uploads per hotel

## Fallback conventional routes

- `/Controller/Action/{id?}` is still available via the default route in `Program.cs`.
