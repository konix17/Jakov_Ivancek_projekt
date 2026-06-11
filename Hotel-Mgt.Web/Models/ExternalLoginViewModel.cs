using System.ComponentModel.DataAnnotations;

namespace HotelMgt.Web.Models;

public class ExternalLoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(11, MinimumLength = 11)]
    [RegularExpression("^[0-9]*$", ErrorMessage = "OIB smije sadržavati samo brojeve.")]
    [Display(Name = "OIB")]
    public string OIB { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
