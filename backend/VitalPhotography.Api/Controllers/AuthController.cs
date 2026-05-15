using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace VitalPhotography.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IConfiguration config) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var hash = config["Admin:PasswordHash"] ?? string.Empty;
        if (!BCrypt.Net.BCrypt.Verify(req.Password, hash))
            return Unauthorized(new { error = "Invalid credentials" });

        var token = GenerateToken();
        return Ok(new { token });
    }

    private string GenerateToken()
    {
        var secret = config["Jwt:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = int.TryParse(config["Jwt:ExpiryMinutes"], out var mins) ? mins : 60;

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: [new Claim(ClaimTypes.Role, "Admin")],
            expires: DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Password);
