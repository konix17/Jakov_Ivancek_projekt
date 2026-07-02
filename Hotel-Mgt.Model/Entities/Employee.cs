using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelMgt.Model.Enums;

namespace HotelMgt.Model.Entities;

public class Employee
{
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Salary { get; set; }
    public EmployeeRole Role { get; set; }
    public DateTime HireDate { get; set; }

    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual Hotel Hotel { get; set; }
    
    public Employee()
    {
        FirstName = null!;
        LastName = null!;
        Email = null!;
        PhoneNumber = null!;
        Hotel = null!;
    }
}