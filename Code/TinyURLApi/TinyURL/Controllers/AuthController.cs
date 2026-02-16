using BCrypt.Net;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TinyURL.Data;
using TinyURL.DTOs;
using TinyURL.Models;
using TinyURL.Services;

namespace TinyURL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration,
            EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // ===============================
        // REGISTER
        // ===============================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower()))
            {
                return BadRequest("Email already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            string verificationToken = Guid.NewGuid().ToString();

            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = passwordHash,
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var verificationLink =
                $"http://localhost:5173/verify?token={verificationToken}";

            var emailBody = $@"
                <h3>Verify Your Email</h3>
                <p>Please click below link:</p>
                <a href='{verificationLink}'>Verify Email</a>";

            await _emailService.SendEmailAsync(
                user.Email,
                "Verify Your Email - TinyURL",
                emailBody
            );

            return Ok("Registration successful. Please verify your email.");
        }

        // ===============================
        // VERIFY EMAIL
        // ===============================
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

            if (user == null)
                return BadRequest("Invalid token.");

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;

            await _context.SaveChangesAsync();

            return Ok("Email verified successfully.");
        }

        // ===============================
        // LOGIN
        // ===============================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!user.IsEmailVerified)
                return Unauthorized("Please verify your email first.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            return Ok(GenerateJwtToken(user));
        }

        // ===============================
        // GOOGLE LOGIN
        // ===============================
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                request.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] }
                });

            var email = payload.Email.ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    GoogleId = payload.Subject,
                    IsGoogleAccount = true,
                    IsEmailVerified = true,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return Ok(GenerateJwtToken(user));
        }

        // ===============================
        // FORGOT PASSWORD
        // ===============================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null)
                return Ok("If account exists, reset link sent.");

            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            var resetLink =
                $"http://localhost:5173/reset-password?email={user.Email}&token={user.PasswordResetToken}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Password - TinyURL",
                $"Click here: <a href='{resetLink}'>Reset Password</a>"
            );

            return Ok("Reset link sent.");
        }

        // ===============================
        // RESET PASSWORD
        // ===============================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == model.Email.ToLower());

            if (user == null)
                return BadRequest("Invalid request.");

            if (user.PasswordResetToken != model.Token)
                return BadRequest("Invalid token.");

            if (user.ResetTokenExpiry == null ||
                user.ResetTokenExpiry < DateTime.UtcNow)
                return BadRequest("Token expired.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Password reset successful.");
        }

        // ===============================
        // ADMIN ONLY - GET USERS
        // ===============================
        [Authorize(Roles = "Admin")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Role,
                    u.IsEmailVerified
                })
                .ToListAsync();

            return Ok(users);
        }

        // ===============================
        // ADMIN ONLY - UPDATE ROLE
        // ===============================
        [Authorize(Roles = "Admin")]
        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateUserRole(int userId, string newRole)
        {
            var allowedRoles = new[] { "Admin", "User" };

            if (!allowedRoles.Contains(newRole))
                return BadRequest("Invalid role.");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return Ok("Role updated successfully.");
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            // Prevent deleting yourself
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == user.Id.ToString())
                return BadRequest("You cannot delete yourself.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

        // ===============================
        // JWT GENERATOR
        // ===============================
        private object GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwtSettings["DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            };
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Email,
                user.FullName,
                user.CreatedAt
            });
        }
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
                return NotFound();

            user.FullName = model.FullName;

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully.");
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (model == null)
                return BadRequest("Invalid request.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
                return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
                return NotFound();

            // Delete user's URLs
            var userLinks = _context.UrlMappings
                .Where(u => u.UserId == user.Id);

            _context.UrlMappings.RemoveRange(userLinks);

            // Delete user
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("Account deleted successfully.");
        }


    }
}
