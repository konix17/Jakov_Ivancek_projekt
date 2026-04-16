using Lab1.Model.Enums;

namespace Lab1.Model.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Salary { get; set; }
    public EmployeeRole Role { get; set; }
    public DateTime HireDate { get; set; }

    public Hotel Hotel { get; set; }
}