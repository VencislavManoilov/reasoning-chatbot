using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
    [HttpPost("login")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public IActionResult Login([FromForm] LoginRequest request) {
        return Ok(new { message = "Login successful", username = request.Username, password = request.Password });
    }

    [HttpPost("register")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    public IActionResult Register([FromForm] RegisterRequest request) {
        
        return Ok(new { message = "Registration successful", username = request.Username, email = request.Email, password = request.Password });
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