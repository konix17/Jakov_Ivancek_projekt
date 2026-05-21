# Hotel-Mgt Completion Report

## Hotel-Mgt Task Breakdown

### 1. Kreiranje kompletno funkcionalne CRUD podrške za sve entitete
- What I did:
  - Implemented CRUD actions in controllers for `Hotel`, `Room`, `Service`, `Employee`, `Guest`, `Reservation`, `Payment`, and `Review`.
  - Added or completed repository methods in `Hotel-Mgt.Web/Repositories/IHotelRepository.cs` and `Hotel-Mgt.Web/Repositories/EfHotelRepository.cs`.
- Where it is:
  - Controllers: `Hotel-Mgt.Web/Controllers/*.cs`
  - Repository interface: `Hotel-Mgt.Web/Repositories/IHotelRepository.cs`
  - Repository implementation: `Hotel-Mgt.Web/Repositories/EfHotelRepository.cs`
- How it works:
  - Each controller has `Index`, `Create`, `Edit`, `Delete`, and `Details` actions.
  - GET actions load lookup data and pass models to views.
  - POST actions validate `ModelState`, save changes, and redirect back to `Index`.

### 2. Kreiranje padajućeg izbornika s AJAX autocomplete opcijom pretrage
- What I did:
  - Built a reusable partial view for autocomplete dropdowns.
  - Added autocomplete endpoints on controllers and client-side AJAX code.
- Where it is:
  - Partial view: `Hotel-Mgt.Web/Views/Shared/_AutocompleteDropdown.cshtml`
  - JS helper: `Hotel-Mgt.Web/wwwroot/js/site.js`
  - Autocomplete endpoints: `Hotel-Mgt.Web/Controllers/GuestsController.cs`, `RoomsController.cs`, `ServicesController.cs`, `EmployeesController.cs`, `ReviewsController.cs`, `ReservationsController.cs`
- How it works:
  - User types in the autocomplete input and the JS sends `GET` to the controller endpoint.
  - The controller returns JSON result items with `id`, `text`, and metadata.
  - JS renders suggestion items and stores the selected ID in a hidden input.

### 3. Implementacija validacije (client side + server side)
- What I did:
  - Added data annotation attributes to form models.
  - Ensured views display validation messages and the layout includes validation scripts.
  - Added explicit `data-val` attributes to custom date picker inputs where Razor helper support was unavailable.
  - Controllers validate `ModelState` in POST actions.
- Where it is:
  - Form models: `Hotel-Mgt.Web/Models/FormModels.cs`
  - Views: `Hotel-Mgt.Web/Views/*/*Form.cshtml`, `Hotel-Mgt.Web/Views/Shared/_DateTimePicker.cshtml`, and `Hotel-Mgt.Web/Views/Shared/_ValidationScriptsPartial.cshtml`
  - Layout: `Hotel-Mgt.Web/Views/Shared/_Layout.cshtml`
- How it works:
  - Client-side validation is driven by unobtrusive validation attributes rendered by Razor helpers and explicit `data-val` markup.
  - The custom date picker also triggers validation on `blur` and on closing the Flatpickr popup via `input.valid()` in `wwwroot/js/site.js`.
  - Server-side validation runs in POST actions using `ModelState.IsValid`; invalid models are re-rendered with error messages.

### 4. Napredno korištenje JavaScripta
- What I did:
  - Implemented debounced AJAX search with smooth fade-in updates.
  - Created autocomplete dropdown logic with open-on-focus behavior.
  - Added hotel-dependent room filtering on reservation forms.
  - Built a custom date picker popup for date inputs.
  - Added client-side validation triggers and fade-in animations for dynamic content.
- Where it is:
  - JavaScript: `Hotel-Mgt.Web/wwwroot/js/site.js`
  - AJAX-enabled index page forms: `Hotel-Mgt.Web/Views/*/Index.cshtml`
  - Reservation hotel/room filtering: `Hotel-Mgt.Web/Views/Reservations/_ReservationForm.cshtml`
- How it works:
  - Search forms submit via AJAX when the user stops typing, updating partial views without full reload.
  - Autocomplete opens suggestions on focus and fetches matching items from the server.
  - Hotel selection triggers an AJAX request to `ReservationsController.HotelRooms`, then repopulates the room dropdown.
  - Dynamic partials and list updates animate with `.fadeIn(250)` in `site.js` so content appears smoothly.

### 5. Datumska kontrola (partial view)
- What I did:
  - Added a custom shared date picker partial view.
  - Replaced default `<input type="date">` behavior with a custom JS-driven picker.
  - Updated review and payment create actions to assign today's date automatically so users do not need to pick the creation/payment date manually.
- Where it is:
  - Partial view: `Hotel-Mgt.Web/Views/Shared/_DateTimePicker.cshtml`
  - JS logic: `Hotel-Mgt.Web/wwwroot/js/site.js`
  - Review and payment forms: `Hotel-Mgt.Web/Views/Reviews/_ReviewForm.cshtml` and `Hotel-Mgt.Web/Views/Payments/_PaymentForm.cshtml`
- How it works:
  - The partial renders a text input plus a popup calendar.
  - JS parses and formats dates in `dd.MM.yyyy.` style, supporting browser locale parsing.
  - The picker writes the selected date back to the form input and triggers validation on `blur`.
  - Review/payment creation now uses `DateTime.Today` in controller POST actions, while edit forms still allow manual date adjustment.

## Additional details
- Reservation-specific improvements:
  - Added `HotelId` to `ReservationFormModel` and filtered `RoomId` options by selected hotel.
  - Updated `Hotel-Mgt.Web/Views/Reservations/Details.cshtml` to show hotel name and correct payments/services counts.
- Log generation fix:
  - Updated `.github/hooks/settings.json` to use a shell-based stop hook for macOS.
  - Added `.github/hooks/stop_hook.sh` and `.github/hooks/stop_hook.py` so session logs can be generated on macOS/Linux.

## Notes
- This report focuses on the Hotel-Mgt checklist tasks, with exact file locations and behavioral description for each task.
- The code now supports the full Hotel-Mgt requirements in the current `Hotel-Mgt.Web` project.
