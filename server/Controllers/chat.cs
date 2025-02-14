using Microsoft.AspNetCore.Mvc;
using App.Data;
using Microsoft.EntityFrameworkCore;
using App.Models;
using OpenAI.Chat;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatContoller : ControllerBase {
    private readonly AppDbContext _context;

    public ChatContoller(AppDbContext context) {
        _context = context;
    }

    [Route("create")]
    public async Task<IActionResult> CreateChat([FromForm] ChatRequest request) {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || !int.TryParse(userId, out int userIdInt)) {
            return Unauthorized(new { error = "Invalid token" });
        }
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ChatCompletion completion = await client.CompleteChatAsync(request.Message);
        return Ok(new { message = completion.Content[0].Text, completion });
    }
}

public class ChatRequest {
    public required string Message { get; set; }
}