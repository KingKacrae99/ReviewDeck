using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace ReviewDeck.Models;

public class User
{
    public int Id { get; set;}

    public string Firstname { get; set;} = string.Empty;

    public string Lastname { get; set;} = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set;}

    public string PasswordHash { get; set;} = string.Empty;

    [Required(ErrorMessage = "Role is  required.")]
    [AllowedValues("admin", "staff", "client")]
    public string Role { get; set;} = "client";
    public ICollection<JobApplication> JobApplications { get; set; }
    = new List<JobApplication>();
    public string Slug {get; set;} = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Helper property to get the full name
    public string FullName => $"{Firstname} {Lastname}".Trim();

    // Method to generate a slug 
    public string BuildSlug()
    {
        if(string.IsNullOrWhiteSpace(Firstname) && string.IsNullOrWhiteSpace(Lastname))
        {
            return string.Empty;
        }

        string raw = $"{Firstname}-{Lastname}".Trim().ToLowerInvariant();

        // Remove non alphanumeric characters except spaces and hyphens
        string slug = Regex.Replace(raw,@"[^a-z0-9\s-]", "");

         // Replace multiple spaces or hyphens with a single hyphen
         slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');
        return slug;
    }

    public void SetForCreation()
    {
        CreatedAt = DateTime.UtcNow;
        Slug = BuildSlug();
    }
}