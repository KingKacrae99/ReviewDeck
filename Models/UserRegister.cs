using System.ComponentModel.DataAnnotations;
namespace ReviewDeck.Models;

public class UserRegisterModel
{
    [Required(ErrorMessage = "Firstname is required.")]
    [MinLength(2, ErrorMessage = "Firstname must be at least 2 characters long.")]
    [MaxLength(50, ErrorMessage = "Firstname cannot exceed 50 characters.")]
    public string Firstname { get; set;} = string.Empty;

    [Required(ErrorMessage = "Lastname is required.")]
    [MinLength(2, ErrorMessage = "Lastname must be at least 2 characters long.")]
    [MaxLength(50, ErrorMessage = "Lastname cannot exceed 50 characters.")]
    public string Lastname { get; set;} = string.Empty;

    [Required(ErrorMessage = "Email is required."), 
    EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    [MinLength(8, ErrorMessage = "Phone number must be at least 8 characters long.")]
    [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
    public string? Phone { get; set;}

    [Required(ErrorMessage = "Password is required.")]
    [Length(8, 100, ErrorMessage = "Password must be between 8 and 100 characters long.")]
    [DataType(DataType.Password)]
    public string Password { get; set;} = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required.")]
    [Length(8, 100, ErrorMessage = "Confirm Password must be between 8 and 100 characters long.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set;} = string.Empty;
}