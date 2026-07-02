using System.ComponentModel.DataAnnotations;

namespace HotelMgt.Model.Entities;

public class Guest
{
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string DocumentNumber { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }

    public Guest()
    {
        Reservations = new List<Reservation>();
        Reviews = new List<Review>();

        FirstName = null!;
        LastName = null!;
        Email = null!;
        PhoneNumber = null!;
        DocumentNumber = null!;
    }
}