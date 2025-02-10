using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Data;
using Microsoft.EntityFrameworkCore;
using App.Models;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
    private string HashPassword(string password) {
        // Implement your password hashing logic here
        using (var sha256 = System.Security.Cryptography.SHA256.Create()) {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    private readonly AppDbContext _context;

    public AuthController(AppDbContext context) {
        _context = context;
    }

    [HttpPost("login")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public IActionResult Login([FromForm] LoginRequest request) {
        return Ok(new { message = "Login successful", username = request.Username, password = request.Password });
    }

    [HttpPost("register")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Name == request.Username)) {
            return BadRequest("Username already exists");
        }

        if (await _context.Users.AnyAsync(u => u.Email == request.Email)) {
            return BadRequest("Email already exists");
        }

        // Create new user
        var user = new User {
            Name = request.Username,
            Email = request.Email,
            Password = HashPassword(request.Password)
        };

        // Add to database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful" });
    }

    [HttpDelete("delete")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public IActionResult Delete([FromForm] DeleteRequest request) {
        return Ok(new { message = "Account deleted successfully", username = request.Username, password = request.Password });
    }
}

public class LoginRequest {
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class RegisterRequest {
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}

public class DeleteRequest {
    public required string Username { get; set; }
    public required string Password { get; set; }
}