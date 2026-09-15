using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppSecLab.Api.Data;
using Microsoft.AspNetCore.Identity;
using AppSecLab.Api.Models;


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
        // FIXED: parameterized query + PasswordHasher<User> for verification (no plaintext comparison)
        var sql = "SELECT Id, Username, PasswordHash FROM Users WHERE Username = @Name";
        

        using var connection = _db.Database.GetDbConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@Name";
        nameParam.Value = request.Username;
        command.Parameters.Add(nameParam);
        

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var storedHash = reader["PasswordHash"].ToString();
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(new User(), storedHash, request.Password);
            if (result == PasswordVerificationResult.Success)
            {
                return Ok(new { username = reader["Username"].ToString(), message = "Login successful" });
            }
        }

        return Unauthorized(new { message = "Invalid credentials" });
        }
    }
