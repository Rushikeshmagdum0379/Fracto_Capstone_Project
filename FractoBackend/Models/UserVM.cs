namespace FractoBackend.Models;

public partial class UserVM
{
    // public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? PhoneNo { get; set; }

    public string Role { get; set; } = "User";   // default role
    public string City { get; set; } = null!;

    // File upload
    public IFormFile? ProfileImage { get; set; }
    
}