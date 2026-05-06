using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lab3.Model.Enums;

namespace Lab3.Model.Entities;

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
}