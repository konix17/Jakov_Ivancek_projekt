namespace Lab1.Model.Entities;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }

    public Hotel Hotel { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Service()
    {
        Reservations = new List<Reservation>();
    }
}