using Microsoft.AspNetCore.Mvc;
using App.Data;
using Microsoft.EntityFrameworkCore;
using App.Models;
using OpenAI.Chat;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatContoller : ControllerBase {
    private readonly AppDbContext _context;

    public ChatContoller(AppDbContext context) {
        _context = context;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List() {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || !int.TryParse(userId, out int userIdInt)) {
            return Unauthorized(new { error = "Invalid token" });
        }

        var chats = await _context.Chats
            .Where(c => c.UserId == userIdInt)
            .OrderByDescending(c => c.Id)
            .Select(c => new { c.Id, c.Title })
            .ToListAsync();
        return Ok(chats);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat([FromForm] ChatRequest request) {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || !int.TryParse(userId, out int userIdInt)) {
            return Unauthorized(new { error = "Invalid token" });
        }
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ChatCompletion completion = await client.CompleteChatAsync(request.Message);
        return Ok(new { message = completion.Content[0].Text, completion });
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromForm] SendRequest request) {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || !int.TryParse(userId, out int userIdInt)) {
            return Unauthorized(new { error = "Invalid token" });
        }

        try {
            if(request.ChatId == null) {
                var messages = new List<Message> {
                    new Message { Role = "system", Content = "You are a helpful assistant called Ducky!" },
                    new Message { Role = "user", Content = JsonConvert.DeserializeObject<Message[]>(request.Messages)?.Last().Content }
                };

                ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                string messagesJson = JsonConvert.SerializeObject(messages);
                ChatCompletion completion = await client.CompleteChatAsync(messagesJson);
                
                var completionContent = JsonConvert.DeserializeObject<Message>(completion.Content[0].Text)?.Content;
                messages.Add(new Message { Role = "assistant", Content = completionContent });

                var _chat = new Chat {
                    UserId = userIdInt,
                    Messages = messages != null ? JsonConvert.SerializeObject(messages) : null
                };

                _context.Chats.Add(_chat);
                await _context.SaveChangesAsync();
                request.ChatId = _chat.Id;

                return Ok(new { chatId = _chat.Id, messages });
            } else {
                var chat = await _context.Chats.FindAsync(request.ChatId);
                if(chat == null) {
                    return NotFound(new { error = "Chat not found" });
                }

                if(chat.UserId != userIdInt) {
                    return Unauthorized(new { error = "You do not have permission to access this chat" });
                }

                var messages = JsonConvert.DeserializeObject<List<Message>>(chat.Messages ?? "[]") ?? [];

                messages.Add(new Message { Role = "user", Content = JsonConvert.DeserializeObject<Message[]>(request.Messages)?.Last().Content });

                ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                string messagesJson = JsonConvert.SerializeObject(messages);
                ChatCompletion completion = await client.CompleteChatAsync(messagesJson);

                var completionContent = completion.Content[0].Text;
                messages.Add(new Message { Role = "assistant", Content = completionContent });

                chat.Messages = JsonConvert.SerializeObject(messages);
                _context.Chats.Update(chat);
                await _context.SaveChangesAsync();

                return Ok(new { messages });
            }
        } catch(Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet("view")]
    public async Task<IActionResult> View([FromQuery] IdRequest request) {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || !int.TryParse(userId, out int userIdInt)) {
            return Unauthorized(new { error = "Invalid token" });
        }

        var chat = await _context.Chats.FindAsync(request.ChatId);
        if(chat == null) {
            return NotFound(new { error = "Chat not found" });
        }

        if(chat.UserId != userIdInt) {
            return Unauthorized(new { error = "You do not have permission to access this chat" });
        }

        var messages = JsonConvert.DeserializeObject<List<Message>>(chat.Messages ?? "[]") ?? [];

        return Ok(new { messages });
    }

    [HttpPost("give-title")]
    public async Task<IActionResult> GiveTitle([FromForm] IdRequest request) {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || !int.TryParse(userId, out int userIdInt)) {
            return Unauthorized(new { error = "Invalid token" });
        }

        var chat = await _context.Chats.FindAsync(request.ChatId);
        if(chat == null) {
            return NotFound(new { error = "Chat not found" });
        }

        if(chat.UserId != userIdInt) {
            return Unauthorized(new { error = "You do not have permission to access this chat" });
        }

        var messages = JsonConvert.DeserializeObject<List<Message>>(chat.Messages ?? "[]") ?? [];

        if (messages.Count > 0) {
            messages[0].Content = "Based on the conversasion below, make a title for the conversasion.";
        }

        ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        string messagesJson = JsonConvert.SerializeObject(messages);
        ChatCompletion completion = await client.CompleteChatAsync(messagesJson);

        chat.Title = completion.Content[0].Text;
        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();

        return Ok(new { title = chat.Title });
    }
}

public class ChatRequest {
    public required string Message { get; set; }
}

public class SendRequest {
    public required string Messages { get; set; }
    public int? ChatId { get; set; }
}

public class IdRequest {
    public int ChatId { get; set; }
}

public class Message {
    public string? Role { get; set; }
    public string? Content { get; set; }
}