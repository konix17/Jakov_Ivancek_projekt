using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelMgt.Model.Enums;

namespace HotelMgt.Model.Entities;

public class Payment
{
    [Key]
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsPaid { get; set; }

    [ForeignKey("Reservation")]
    public int ReservationId { get; set; }
    public virtual Reservation Reservation { get; set; }

    public Payment()
    {
        Reservation = null!;
    }
}