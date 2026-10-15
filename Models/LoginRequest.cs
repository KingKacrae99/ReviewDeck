using System.ComponentModel.DataAnnotations;
namespace ReviewDeck.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email {get; set; } = string.Empty;

    [Required (ErrorMessage ="This field is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password {get; set; } = string.Empty;
}