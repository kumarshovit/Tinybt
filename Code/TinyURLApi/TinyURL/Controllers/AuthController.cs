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

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u =>
                u.Email.ToLower() == request.Email.ToLower()))
                return BadRequest("Email already exists.");

            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User",
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
                return Unauthorized("Invalid credentials.");

            // 🔒 Check if account is locked
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                return Unauthorized(
                  $"Account locked until {user.LockoutEnd.Value.ToLocalTime():dd-MM-yyyy HH:mm:ss}"
                 );

            }

            // 🔑 Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                var maxAttempts = int.Parse(_configuration["AuthSettings:MaxFailedAttempts"]!);
                var lockoutMinutes = int.Parse(_configuration["AuthSettings:LockoutMinutes"]!);

                if (user.FailedLoginAttempts >= maxAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                    user.FailedLoginAttempts = 0; // reset counter
                }

                await _context.SaveChangesAsync();

                Console.WriteLine($"⚠️ Suspicious login attempt for {user.Email} from IP: {HttpContext.Connection.RemoteIpAddress}");

                return Unauthorized("Invalid credentials.");
            }

            // ✅ Successful login → reset counters
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            var (accessToken, expires) = GenerateJwtToken(user);

            // Generate refresh token
            var refreshToken = Guid.NewGuid().ToString();

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken,
                refreshToken,
                expires
            });
        }


        // ================= REFRESH TOKEN =================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var storedToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == model.RefreshToken);

            if (storedToken == null ||
                storedToken.IsRevoked ||
                storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token.");
            }

            // Revoke old refresh token
            storedToken.IsRevoked = true;

            // Generate new tokens
            var (newAccessToken, expires) = GenerateJwtToken(storedToken.User);

            var newRefreshToken = Guid.NewGuid().ToString();

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken,
                expires
            });
        }


        // ================= LOGOUT =================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto model)
        {
            var accessToken = Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            // Revoke access token
            _context.RevokedTokens.Add(new RevokedSession
            {
                Token = accessToken,
                RevokedAt = DateTime.UtcNow
            });

            // Revoke refresh token
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == model.RefreshToken);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
            }

            await _context.SaveChangesAsync();

            return Ok("Logged out successfully.");
        }


        // ================= PROFILE =================
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            return Ok(new
            {
                user.Email,
                user.FullName,
                user.CreatedAt
            });
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            user.FullName = model.FullName;
            await _context.SaveChangesAsync();

            return Ok("Profile updated.");
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }

        // ================= ADMIN =================
        [Authorize(Roles = "Admin")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Role
                })
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return Ok("Role updated.");
        }
        // ================= ADMIN DELETE USER =================
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found.");

            var links = _context.UrlMappings
                .Where(u => u.UserId == user.Id);

            _context.UrlMappings.RemoveRange(links);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

        // ================= ACCOUNT DELETE =================
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            var links = _context.UrlMappings
                .Where(u => u.UserId == user.Id);

            _context.UrlMappings.RemoveRange(links);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("Account deleted.");
        }
        // ================= FORGOT PASSWORD =================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null)
                return Ok("If account exists, reset link has been sent.");

            var resetToken = Guid.NewGuid().ToString();

            var resetLink = $"https://localhost:5173/reset-password?email={user.Email}&token={resetToken}";

            // Send email
            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Your Password",
                $"Click the link below to reset your password:\n\n{resetLink}"
            );

            return Ok("If account exists, reset link has been sent.");
        }

        // ================= RESET PASSWORD =================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null)
                return BadRequest("Invalid request.");

            // 🔐 Normally validate token from DB here

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            await _context.SaveChangesAsync();

            return Ok("Password reset successful.");
        }


        // ================= JWT GENERATOR =================
        private (string token, DateTime expires) GenerateJwtToken(User user)
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

            return (
                new JwtSecurityTokenHandler().WriteToken(token),
                token.ValidTo
            );
        }
    }
}
