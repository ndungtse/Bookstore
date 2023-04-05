

using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models;

public class LoginModel : IValidatableObject
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Email == " " && Password == " ")
        {
            yield return new ValidationResult(
                "Email and password are required",
                new[] { nameof(Email), nameof(Password) });
        }
        else if (Email == " ")
        {
            yield return new ValidationResult(
                "Email is required",
                new[] { nameof(Email) });
        }
        else if (Password == " ")
        {
            yield return new ValidationResult(
                "Password is required",
                new[] { nameof(Password) });
        }

    }
}