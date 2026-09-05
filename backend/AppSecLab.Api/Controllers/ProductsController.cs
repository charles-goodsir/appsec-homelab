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
    // FIXED: parameterized query — wildcards applied to the parameter value, not the SQL text
        var sql = "SELECT Id, Name, Description FROM Products WHERE Name LIKE @Query";

        using var connection = _db.Database.GetDbConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        var  queryParam = command.CreateParameter();
        queryParam.ParameterName = "@Query";
        queryParam.Value = $"%{query}%";         
        command.Parameters.Add(queryParam);
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