using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMgt.Model.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey("Guest")]
    public int GuestId { get; set; }
    public virtual Guest Guest { get; set; }

    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual Hotel Hotel { get; set; }
}