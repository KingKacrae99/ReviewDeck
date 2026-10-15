using System.ComponentModel.DataAnnotations;
 namespace ReviewDeck.Models;

 public class JobApplication
{
    public int Id { get; set; }

    public int UserId { get; set;}
    public User User { get; set; } = null!;

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(150, MinimumLength = 2, 
    ErrorMessage = "Job title must be between 2 and 150 characters.")]
    public string JobTitle { get; set;} = string.Empty;

    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(150, MinimumLength = 2,
    ErrorMessage = "company must be between 2 and 150 characters.")]
    public string Company { get; set;} = string.Empty;

    [StringLength(1000, MinimumLength = 10,
    ErrorMessage = "Job Description must be between 10 and 1000 characters.")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set;}

    [Required(ErrorMessage = "Job URL is required.")]
    [Url(ErrorMessage = "Invalid URL format.")]
    public string JobUrl { get; set;} = string.Empty;

    [StringLength(150, MinimumLength = 5,
    ErrorMessage = "Location must be between 5 and 150 characters.")]
    [DataType(DataType.Text, ErrorMessage = "Invalid location format.")]
    public string? Location { get; set;}

    [AllowedValues("Applied", "Interview", "Rejected", "Offered")]
    public string Status { get; set;} = "Applied";
    [StringLength(100, MinimumLength = 3)]
    public string? Platform { get; set;}

    [AllowedValues("Part-Time","Fulltime", "Contract", "Internship", "Freelance")]
    public string JobType { get; set;} = "Fulltime";

    [AllowedValues("Remote", "Onsite", "Hybrid")]
    public string JobMode { get; set;} = "Remote";

    public DateTime AppliedAt {get; set;}
    public DateTime CreatedAt { get; set;}
    public DateTime UpdatedAt { get; set;}

    public void SetForCreation()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetForUpdate()
    {
        UpdatedAt = DateTime.UtcNow;
    }

}