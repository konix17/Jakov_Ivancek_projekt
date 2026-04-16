namespace Lab1.Model.Entities;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guest Guest { get; set; }
    public Hotel Hotel { get; set; }
}