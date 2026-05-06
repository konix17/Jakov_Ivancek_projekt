# Semantic Model

## Model classes and tables

- Hotel
  - `Id`, `Name`, `Address`, `City`, `Rating`, `PhoneNumber`
  - One-to-many: `Rooms`, `Employees`, `Services`, `Reservations`, `Reviews`

- Room
  - `Id`, `RoomNumber`, `Floor`, `Capacity`, `PricePerNight`, `RoomType`, `IsAvailable`, `HotelId`
  - Many-to-one: `Hotel`
  - One-to-many: `Reservations`

- Service
  - `Id`, `Name`, `Description`, `Price`, `IsAvailable`, `HotelId`
  - Many-to-one: `Hotel`
  - Many-to-many: `Reservations`

- Employee
  - `Id`, `FirstName`, `LastName`, `Email`, `PhoneNumber`, `Salary`, `Role`, `HireDate`, `HotelId`
  - Many-to-one: `Hotel`

- Guest
  - `Id`, `FirstName`, `LastName`, `Email`, `PhoneNumber`, `DateOfBirth`, `DocumentNumber`
  - One-to-many: `Reservations`, `Reviews`

- Reservation
  - `Id`, `ReservationCode`, `ReservationDate`, `CheckInDate`, `CheckOutDate`, `TotalPrice`, `Status`, `GuestId`, `RoomId`
  - Many-to-one: `Guest`, `Room`
  - Many-to-many: `Services`
  - One-to-many: `Payments`

- Payment
  - `Id`, `Amount`, `PaymentDate`, `PaymentMethod`, `IsPaid`, `ReservationId`
  - Many-to-one: `Reservation`

- Review
  - `Id`, `Rating`, `Comment`, `CreatedAt`, `GuestId`, `HotelId`
  - Many-to-one: `Guest`, `Hotel`

## Notes

- The database model is based on hotel management with reservations, guests, employees, services, payments and reviews.
- `HotelDbContext` maps all eight entity types into database tables.
- `Reservation` and `Service` form a many-to-many relationship.
