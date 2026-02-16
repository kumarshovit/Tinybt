using System.ComponentModel.DataAnnotations;

namespace TinyURL.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain at least 8 characters, including uppercase, lowercase, number, and special character."
        )]
        public string Password { get; set; }
    }
}
