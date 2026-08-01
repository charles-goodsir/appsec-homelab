using Microsoft.AspNetCore.Mvc;
using AppSecLab.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AppSecLab.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;
  [HttpGet("search")]
  public IActionResult Search([FromQuery] string query)
  {
    // VULNERABLE: raw string concatenation into SQL (A05:2025 - Injection)
        var sql = $"SELECT Id, Name, Description FROM Products WHERE Name LIKE '%{query}%'";

        using var connection = _db.Database.GetDbConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        using var reader = command.ExecuteReader();

        var results = new List<object>();
        while (reader.Read())
        {
            results.Add(new
            {
                id = reader["Id"],
                name = reader["Name"].ToString(),
                description = reader["Description"].ToString()
            });
        }

        return Ok(results);
    }
}