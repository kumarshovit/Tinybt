using System.ComponentModel.DataAnnotations;

namespace TinyURL.Models
{
    public class User
    {
        public string? FullName { get; set; }

        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public string? EmailVerificationToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int FailedLoginAttempts { get; set; } = 0;

        public bool IsLocked { get; set; } = false;

        public DateTime? LockoutEnd { get; set; }
        public string? GoogleId { get; set; }
        public bool IsGoogleAccount { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public string Role { get; set; } = "User";

    }
}
