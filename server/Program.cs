using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Data;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using Npgsql;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var key = Environment.GetEnvironmentVariable("JWT_SECRET");
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? ""))
        };
    }
);


builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options => {
    options.AddPolicy(
        "AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<JwtService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => new { version = "0.1", message = "Welcome to Reasoning Chatbot API" });

app.Run();