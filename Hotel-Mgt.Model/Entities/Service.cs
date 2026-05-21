using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMgt.Model.Entities;

public class Service
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }

    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual Hotel Hotel { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; }

    public Service()
    {
        Reservations = new List<Reservation>();
        Name = null!;
        Description = null!;
        Hotel = null!;
    }
}