using Microsoft.AspNetCore.Mvc;
using App.Data;
using Microsoft.EntityFrameworkCore;
using App.Models;
using OpenAI.Chat;

[ApiController]
[Route("api/chat")]
public class ChatContoller : ControllerBase {
    private readonly AppDbContext _context;

    public ChatContoller(AppDbContext context) {
        _context = context;
    }

    [Route("create")]
    [UseAuthenticationMiddleware]
    public async Task<IActionResult> CreateChat([FromForm] ChatRequest request) {
        var user = HttpContext.Items["User"] as User;
        if (user == null) {
            return Unauthorized(new { message = "User not found" });
        }
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ChatCompletion completion = await client.CompleteChatAsync(request.Message);
        return Ok(new { message = completion.Content[0].Text, completion });
    }
}

public class ChatRequest {
    public required string Message { get; set; }
}