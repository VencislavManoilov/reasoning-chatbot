using Microsoft.AspNetCore.Mvc;
using App.Data;
using Microsoft.EntityFrameworkCore;
using App.Models;
using OpenAI.Chat;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;

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
            var messages = new List<Message>();

            if (request.ChatId == null) {
                messages.Add(new Message { role = "system", content = "You are a helpful assistant called Ducky!" });
                messages.Add(new Message { role = "user", content = request.Message });

                var newChat = new Chat {
                    UserId = userIdInt,
                    Messages = JsonConvert.SerializeObject(messages)
                };

                _context.Chats.Add(newChat);
                await _context.SaveChangesAsync();
                request.ChatId = newChat.Id;
            } else {
                var chat = await _context.Chats.FindAsync(request.ChatId);
                if (chat == null) {
                    return NotFound(new { error = "Chat not found" });
                }

                if (chat.UserId != userIdInt) {
                    return Unauthorized(new { error = "You do not have permission to access this chat" });
                }

                messages = JsonConvert.DeserializeObject<List<Message>>(chat.Messages ?? "[]") ?? new List<Message>();
                messages.Add(new Message { role = "user", content = request.Message });
            }

            Response.ContentType = "text/event-stream";
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("Chat-Id", request.ChatId.ToString());
            Response.Headers.Append("Access-Control-Expose-Headers", "Chat-Id");

            await StreamChatCompletionAsync(messages, Response.Body);

            var updatedChat = await _context.Chats.FindAsync(request.ChatId);
            if (updatedChat != null) {
                updatedChat.Messages = JsonConvert.SerializeObject(messages);
                _context.Chats.Update(updatedChat);
                await _context.SaveChangesAsync();
            }

            return new EmptyResult();
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    private async Task StreamChatCompletionAsync(List<Message> messages, Stream responseStream) {
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        string messagesJson = JsonConvert.SerializeObject(messages);
        IAsyncEnumerable<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync(messagesJson);

        var assistantMessage = new Message { role = "assistant", content = "" };
        messages.Add(assistantMessage);

        await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates) {
            if (completionUpdate.ContentUpdate.Count > 0) {
                string content = completionUpdate.ContentUpdate[0].Text;
                assistantMessage.content += content;
                await responseStream.WriteAsync(Encoding.UTF8.GetBytes(content));
            }
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

        var messages = JsonConvert.DeserializeObject<List<Message>>(chat.Messages ?? "[]") ?? new List<Message>();

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

        var messages = JsonConvert.DeserializeObject<List<Message>>(chat.Messages ?? "[]") ?? new List<Message>();

        if (messages.Count > 0) {
            messages[0].content = "Based on the conversasion below, make a title for the conversasion.";
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
    public required string Message { get; set; }
    public int? ChatId { get; set; }
}

public class IdRequest {
    public int ChatId { get; set; }
}

public class Message {
    public string? role { get; set; }
    public string? content { get; set; }
}