using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppSecLab.Api.Data;

namespace AppSecLab.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db) => _db = db;

    public record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // VULNERABLE: raw string concatenation into SQL (A05:2025 - Injection)
        // Mirrors the PortSwigger login-bypass lab pattern - never do this in real code.
        var sql = "SELECT Id, Username FROM Users WHERE Username = @Name AND Password = @Password";
        

        using var connection = _db.Database.GetDbConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@Name";
        nameParam.Value = request.Username;
        command.Parameters.Add(nameParam);
        var passwordParam = command.CreateParameter();
        passwordParam.ParameterName = "@Password";
        passwordParam.Value = request.Password;
        command.Parameters.Add(passwordParam);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return Ok(new { username = reader["Username"].ToString(), message = "Login successful" });
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }
}