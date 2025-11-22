using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Tems.Common.Identity;

namespace Tems.IdentityServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMongoCollection<User> _users;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IMongoDatabase database,
        IPasswordHasher<User> passwordHasher,
        ILogger<AccountController> logger)
    {
        _users = database.GetCollection<User>("users");
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Email and password are required" });
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(new { error = "Password must be at least 6 characters long" });
        }

        if (!IsValidEmail(request.Email))
        {
            return BadRequest(new { error = "Invalid email format" });
        }

        try
        {
            var existingUser = await _users.Find(u => 
                u.Email == request.Email || u.Username == request.Username).FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.Email == request.Email)
                {
                    return BadRequest(new { error = "Email already registered" });
                }
                return BadRequest(new { error = "Username already taken" });
            }

            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = request.Username ?? request.Email.Split('@')[0],
                Email = request.Email,
                FullName = request.FullName ?? request.Email.Split('@')[0],
                IsActive = true,
                Roles = new List<string> { "User" },
                Claims = new Dictionary<string, string>
                {
                    { "can_view_entities", "true" }
                },
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _users.InsertOneAsync(user);

            _logger.LogInformation("User registered successfully: {Email}", request.Email);

            return Ok(new
            {
                message = "Registration successful",
                userId = user.Id,
                username = user.Username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user {Email}", request.Email);
            return StatusCode(500, new { error = "Registration failed" });
        }
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { error = "Email is required" });
        }

        var exists = await _users.Find(u => u.Email == email).AnyAsync();
        return Ok(new { available = !exists });
    }

    [HttpGet("check-username")]
    public async Task<IActionResult> CheckUsername([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest(new { error = "Username is required" });
        }

        var exists = await _users.Find(u => u.Username == username).AnyAsync();
        return Ok(new { available = !exists });
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? FullName { get; set; }
}
