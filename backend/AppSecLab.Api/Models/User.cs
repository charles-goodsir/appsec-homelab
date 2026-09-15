namespace AppSecLab.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    //FIXED: password hash storage (A04:2025 - Cryptographic Failures)
    public string PasswordHash { get; set; } = string.Empty;
     
}