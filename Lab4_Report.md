# Hotel-Mgt Completion Report

## Hotel-Mgt Task Breakdown

### 1. Kreiranje kompletno funkcionalne CRUD podrške za sve entitete
- What I did:
  - Implemented CRUD actions in controllers for `Hotel`, `Room`, `Service`, `Employee`, `Guest`, `Reservation`, `Payment`, and `Review`.
  - Added or completed repository methods in `Hotel-Mgt.Web/Repositories/IHotelRepository.cs` and `Hotel-Mgt.Web/Repositories/EfHotelRepository.cs`.
- Where it is:
  - Controllers: `Hotel-Mgt.Web/Controllers/HotelsController.cs`, `Hotel-Mgt.Web/Controllers/RoomsController.cs`, `Hotel-Mgt.Web/Controllers/ServicesController.cs`, `Hotel-Mgt.Web/Controllers/EmployeesController.cs`, `Hotel-Mgt.Web/Controllers/GuestsController.cs`, `Hotel-Mgt.Web/Controllers/ReservationsController.cs`, `Hotel-Mgt.Web/Controllers/PaymentsController.cs`, `Hotel-Mgt.Web/Controllers/ReviewsController.cs`
  - Repository interface: `Hotel-Mgt.Web/Repositories/IHotelRepository.cs`
  - Repository implementation: `Hotel-Mgt.Web/Repositories/EfHotelRepository.cs`
- How it works:
  - Each controller implements `Index`, `Create`, `Edit`, `Delete`, and `Details` routes for the entity.
  - `Create` and `Edit` POST actions always check `ModelState.IsValid` and return the same view when invalid.
  - Repository methods such as `AddEmployee`, `UpdateEmployee`, `DeleteEmployee`, and `SaveChanges` persist changes through EF Core.
  - Search-enabled `Index` pages use repository query methods so the same code path works for full page load and AJAX list updates.

### 2. Kreiranje padajućeg izbornika s AJAX autocomplete opcijom pretrage
- What I did:
  - Built a reusable partial view for autocomplete dropdowns.
  - Added autocomplete endpoints on controllers and client-side AJAX code.
- Where it is:
  - Partial view: `Hotel-Mgt.Web/Views/Shared/_AutocompleteDropdown.cshtml`
  - Autocomplete model: `Hotel-Mgt.Web/Models/UiModels.cs`
  - JS helper: `Hotel-Mgt.Web/wwwroot/js/site.js`
  - Autocomplete endpoints: `Hotel-Mgt.Web/Controllers/HotelsController.cs`, `Hotel-Mgt.Web/Controllers/RoomsController.cs`, `Hotel-Mgt.Web/Controllers/ServicesController.cs`, `Hotel-Mgt.Web/Controllers/EmployeesController.cs`, `Hotel-Mgt.Web/Controllers/GuestsController.cs`, `Hotel-Mgt.Web/Controllers/ReservationsController.cs`, `Hotel-Mgt.Web/Controllers/PaymentsController.cs`, `Hotel-Mgt.Web/Controllers/ReviewsController.cs`
- How it works:
  - `_AutocompleteDropdown.cshtml` renders a visible text input, hidden ID input, results container and validation span.
  - `site.js` initializes each `.autocomplete-dropdown`, debounces user typing, calls the controller endpoint, and fills the dropdown.
  - Controller autocomplete endpoints return JSON objects like `{ id, text, meta }` based on search term.
  - When the user picks an item, the JS sets the text and hidden ID so the form submits the selected foreign key.

### 3. Implementacija validacije (client side + server side)
- What I did:
  - Added data annotation attributes to form models.
  - Ensured views display validation messages and the layout includes validation scripts.
  - Added explicit `data-val` attributes to custom date picker inputs where Razor helper support was unavailable.
  - Controllers validate `ModelState` in POST actions.
- Where it is:
  - Form models: `Hotel-Mgt.Web/Models/FormModels.cs`
  - Form views: `Hotel-Mgt.Web/Views/Employees/_EmployeeForm.cshtml`, `Hotel-Mgt.Web/Views/Rooms/_RoomForm.cshtml`, `Hotel-Mgt.Web/Views/Services/_ServiceForm.cshtml`, `Hotel-Mgt.Web/Views/Guests/_GuestForm.cshtml`, `Hotel-Mgt.Web/Views/Reservations/_ReservationForm.cshtml`, `Hotel-Mgt.Web/Views/Payments/_PaymentForm.cshtml`, `Hotel-Mgt.Web/Views/Reviews/_ReviewForm.cshtml`
  - Shared validation and picker views: `Hotel-Mgt.Web/Views/Shared/_DateTimePicker.cshtml`, `Hotel-Mgt.Web/Views/Shared/_AutocompleteDropdown.cshtml`, `Hotel-Mgt.Web/Views/Shared/_ValidationScriptsPartial.cshtml`
  - Layout: `Hotel-Mgt.Web/Views/Shared/_Layout.cshtml`
- How it works:
  - Form models in `Hotel-Mgt.Web/Models/FormModels.cs` define fields with data annotations such as `[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, and `[Phone]`.
  - Views use `asp-for` and `asp-validation-for` to render inputs and validation spans.
  - `_ValidationScriptsPartial.cshtml` loads jQuery Validation and unobtrusive validation scripts from shared layout.
  - Controller POST actions in entity controllers always check `ModelState.IsValid`, repopulate lookup data via `ViewBag` when invalid, and return the view so validation messages are displayed.

### 4. Napredno korištenje JavaScripta
- What I did:
  - Implemented debounced AJAX search with smooth fade-in updates.
  - Created autocomplete dropdown logic with open-on-focus behavior.
  - Added hotel-dependent room filtering on reservation forms.
  - Built a custom date picker popup for date inputs.
  - Added client-side validation triggers and fade-in animations for dynamic content.
- Where it is:
  - JavaScript: `Hotel-Mgt.Web/wwwroot/js/site.js`
  - AJAX-enabled index page forms: `Hotel-Mgt.Web/Views/Employees/Index.cshtml`, `Hotel-Mgt.Web/Views/Payments/Index.cshtml`, `Hotel-Mgt.Web/Views/Rooms/Index.cshtml`, `Hotel-Mgt.Web/Views/Guests/Index.cshtml`, `Hotel-Mgt.Web/Views/Reservations/Index.cshtml`, `Hotel-Mgt.Web/Views/Reviews/Index.cshtml`, `Hotel-Mgt.Web/Views/Hotels/Index.cshtml`, `Hotel-Mgt.Web/Views/Services/Index.cshtml`
  - Reservation hotel/room filtering: `Hotel-Mgt.Web/Views/Reservations/_ReservationForm.cshtml`
- How it works:
  - `Hotel-Mgt.Web/wwwroot/js/site.js` contains helper functions: `debounce`, `bindAjaxSearchForms`, `bindAutocompleteWidgets`, `bindHotelRoomFilters`, and `initializeDateTimePickers`.
  - `bindAjaxSearchForms` attaches debounced input listeners to `.ajax-search-form`, submits queries to controller `Search` actions, and replaces target partial HTML.
  - `bindHotelRoomFilters` loads rooms dynamically from `ReservationsController.HotelRooms` when the hotel select changes.
  - `bindAutocompleteWidgets` sets up the custom autocomplete dropdown behavior for all forms.
  - `initializeDateTimePickers` applies Flatpickr configuration for date controls and triggers validation after close/blur.

#### AJAX loading chunks
- The AJAX flow has three connected pieces:
  1. the search form in the view (`Hotel-Mgt.Web/Views/Employees/Index.cshtml` and equivalent list views),
  2. the controller `Search` action (`Hotel-Mgt.Web/Controllers/EmployeesController.cs` and equivalent controllers),
  3. the shared AJAX submit handler in `Hotel-Mgt.Web/wwwroot/js/site.js`.
- The view uses `class="ajax-search-form"` and `data-target` to mark the form and the HTML container to update.
- The controller returns a partial view (`_EmployeesTable` for employees) instead of a full page.
- The JS sends the query in the background, receives HTML, and injects it into the target container without reloading the page.

### 5. Datumska kontrola (partial view)
- What I did:
  - Added a custom shared date picker partial view.
  - Replaced default `<input type="date">` behavior with a custom JS-driven picker.
  - Updated review and payment create actions to assign today's date automatically so users do not need to pick the creation/payment date manually.
- Where it is:
  - Partial view: `Hotel-Mgt.Web/Views/Shared/_DateTimePicker.cshtml`
  - JS logic: `Hotel-Mgt.Web/wwwroot/js/site.js`
  - Form views using the picker: `Hotel-Mgt.Web/Views/Employees/_EmployeeForm.cshtml`, `Hotel-Mgt.Web/Views/Guests/_GuestForm.cshtml`, `Hotel-Mgt.Web/Views/Reservations/_ReservationForm.cshtml`, `Hotel-Mgt.Web/Views/Payments/_PaymentForm.cshtml`, `Hotel-Mgt.Web/Views/Reviews/_ReviewForm.cshtml`
- How it works:
  - `_DateTimePicker.cshtml` renders a text input with class `flatpickr-datetime`, `data-val` validation metadata, and a validation message span so the field works with unobtrusive client validation.
  - `site.js` detects all `.flatpickr-datetime` inputs, initializes Flatpickr with locale detection for `hr` or `en`, and sets a custom format instead of native browser date controls.
  - On `blur` and on Flatpickr `onClose`, the script calls `input.valid()` to trigger client validation immediately.
  - The partial is reused on employee hire date, reservation dates, payment date, review date, and other date fields across the app.
  - This ensures date selection works consistently in HR and EN formats and does not depend on browser-native `<input type="date">` behavior.

- Reservation-specific improvements:
  - Added `HotelId` to `ReservationFormModel` and filtered `RoomId` options by selected hotel.
  - Updated `Hotel-Mgt.Web/Views/Reservations/Details.cshtml` to show hotel name and correct payments/services counts.
- Log generation fix:
  - Updated `.github/hooks/settings.json` to use a shell-based stop hook for macOS.
  - Added `.github/hooks/stop_hook.sh` and `.github/hooks/stop_hook.py` so session logs can be generated on macOS/Linux.

## Notes
- This report focuses on the Hotel-Mgt checklist tasks, with exact file locations and behavioral description for each task.
- The code now supports the full Hotel-Mgt requirements in the current `Hotel-Mgt.Web` project.
