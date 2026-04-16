using Lab1.Model.Enums;

namespace Lab1.Model.Entities;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsPaid { get; set; }

    public Reservation Reservation { get; set; }
}