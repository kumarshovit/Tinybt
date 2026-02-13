using Microsoft.AspNetCore.Mvc;
using TinyURL.Data;
using TinyURL.Models;
using TinyURL.DTOs;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace TinyURL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
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
    }
}
