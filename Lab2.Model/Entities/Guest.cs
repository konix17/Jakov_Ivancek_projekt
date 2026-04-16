namespace Lab1.Model.Entities;

public class Guest
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string DocumentNumber { get; set; }

    public List<Reservation> Reservations { get; set; }
    public List<Review> Reviews { get; set; }

    public Guest()
    {
        Reservations = new List<Reservation>();
        Reviews = new List<Review>();
    }
}