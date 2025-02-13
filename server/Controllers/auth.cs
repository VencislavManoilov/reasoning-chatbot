using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Data;
using Microsoft.EntityFrameworkCore;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

    private readonly IConfiguration _configuration;

    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, IConfiguration configuration) {
        _context = context;
        _configuration = configuration;
        _jwtService = new JwtService();
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> Profile() {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null) {
            return Unauthorized(new { error = "Invalid token" });
        }

        var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) {
            return Unauthorized(new { error = "Invalid token" });
        }

        var userId = userIdClaim.Value;
        if (!int.TryParse(userId, out int userIdInt)) {
            return Unauthorized(new { error = "Invalid token" });
        }

        var user = await _context.Users.FindAsync(userIdInt);
        if (user == null) {
            return NotFound(new { error = "User not found" });
        }

        return Ok(new {
            username = user.Name,
            email = user.Email
        });
    }

    [HttpPost("login")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public async Task<IActionResult> Login([FromForm] LoginRequest request) {
        // Find user by username
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        // Check if user exists
        if(user == null) {
            return BadRequest(new { error = "Invalid username or password" });
        }

        // Hash the provided password and compare with stored hash
        string hashedPassword = HashPassword(request.Password);
        if(user.Password != hashedPassword) {
            return BadRequest(new { error = "Invalid username or password" });
        }

        var token = _jwtService.GenerateToken(user.Id.ToString());

        // Login successful
        return Ok(new { 
            message = "Login successful",
            user = new { 
                username = user.Name,
                email = user.Email
            },
            token
        });
    }

    [HttpPost("register")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        // Check if user already exists
        if(await _context.Users.AnyAsync(u => u.Email == request.Email)) {
            return BadRequest(new { error = "Email already exists" });
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

    [HttpPost("logout")]
    public IActionResult Logout() {
        HttpContext.Session.Clear();
        return Ok(new { message = "Logout successful" });
    }

    [HttpDelete("delete")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public async Task<IActionResult> Delete([FromForm] DeleteRequest request) {
        // Find user by username
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == request.Username);
        
        // Check if user exists
        if(user == null) {
            return BadRequest(new { error = "Invalid username or password" });
        }

        // Verify password
        string hashedPassword = HashPassword(request.Password);
        if(user.Password != hashedPassword) {
            return BadRequest(new { error = "Invalid username or password" });
        }

        // Remove user from database
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { 
            message = "Account deleted successfully",
            username = request.Username
        });
    }
}

public class LoginRequest {
    public required string Email { get; set; }
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