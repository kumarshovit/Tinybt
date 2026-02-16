using BCrypt.Net;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
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
                IsEmailVerified = false,   // Production requires verification
                EmailVerificationToken = verificationToken,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // 🔗 Create verification link
            var verificationLink =
                $"http://localhost:5173/verify?token={verificationToken}";

            var emailBody = $@"
        <h3>Verify Your Email</h3>
        <p>Please click the link below to verify your account:</p>
        <a href='{verificationLink}'>Verify Email</a>
    ";

            await _emailService.SendEmailAsync(
                user.Email,
                "Verify Your Email - TinyURL",
                emailBody
            );

            return Ok("Registration successful. Please check your email to verify.");
        }



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

            return Ok("Email verified successfully. You can now login.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!user.IsEmailVerified)
                return Unauthorized("Please verify your email first.");

            if (user.IsLocked && user.LockoutEnd > DateTime.UtcNow)
                return Unauthorized("Account locked. Try again later.");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsLocked = true;
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                }

                await _context.SaveChangesAsync();
                return Unauthorized("Invalid credentials.");
            }

            // Reset failed attempts
            user.FailedLoginAttempts = 0;
            user.IsLocked = false;
            user.LockoutEnd = null;

            await _context.SaveChangesAsync();

            // Generate JWT
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwtSettings["DurationInMinutes"]!)
                ),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
                return BadRequest("IdToken is required.");

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    request.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _configuration["Google:ClientId"] }
                    }
                );

                var email = payload.Email;
                var googleId = payload.Subject;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    user = new User
                    {
                        Email = email.ToLower(),
                        GoogleId = googleId,
                        IsGoogleAccount = true,
                        IsEmailVerified = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Generate JWT
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
                );

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        double.Parse(jwtSettings["DurationInMinutes"]!)
                    ),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expires = token.ValidTo
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized("Invalid Google token.");
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email.ToLower());

            if (user == null)
                return Ok("If account exists, reset link sent.");

            // Generate secure token
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            var resetLink =
                $"http://localhost:5173/reset-password?email={user.Email}&token={user.PasswordResetToken}";

            var emailBody = $@"
        <h3>Reset Password</h3>
        <p>Click below link to reset your password:</p>
        <a href='{resetLink}'>Reset Password</a>
    ";

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Password - TinyURL",
                emailBody
            );

            return Ok("If account exists, reset link sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email.ToLower());

            if (user == null)
                return BadRequest("Invalid request.");

            Console.WriteLine("DB Token: " + user.PasswordResetToken);
            Console.WriteLine("Incoming Token: " + model.Token);

            if (user.PasswordResetToken == null ||
                !user.PasswordResetToken.Equals(model.Token))
            {
                return BadRequest("Invalid token.");
            }

            if (user.ResetTokenExpiry == null ||
                user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest("Token expired.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Password reset successful.");
        }


    }
}
