namespace AppSecLab.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    //VULNERABLE: plaintext password storage (A04:2025 - Cryptographic Failures)
    public string Password { get; set; } = string.Empty;
     
}