using Microsoft.AspNetCore.Mvc;
using TinyURL.Data;
using TinyURL.Models;
using TinyURL.DTOs;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TinyURL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // ✅ Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Check duplicate email (case insensitive)
            if (await _context.Users
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower()))
            {
                return BadRequest("Email already exists.");
            }

            // ✅ Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // ✅ Generate verification token
            string verificationToken = Guid.NewGuid().ToString();

            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = passwordHash,
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully. Please verify your email.",
                verificationToken = verificationToken
            });
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

    }
}
