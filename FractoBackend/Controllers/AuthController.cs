using FractoBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace FractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public AuthController(IUserRepository userRepository, IConfiguration configuration, IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _env = env;
        }
        // Helper: Hash password with SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        // Generate JWT
        // JWT Authentication
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", user.Role),
                new Claim("id", user.UserId.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // POST: api/auth/register
        [HttpPost("register")]
        public IActionResult Register([FromForm] UserVM userVM)
        {
            var existingUser = _userRepository.GetByUsername(userVM.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }
            string? profileImagePath = null;
            //Handle profile image upload
            if (userVM.ProfileImage != null && userVM.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userVM.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    userVM.ProfileImage.CopyTo(stream);
                }
                profileImagePath = "/uploads/" + fileName;
            }
            // Save user with hashed password
            var user = new User
            {
                Username = userVM.Username,
                Password = HashPassword(userVM.Password), // store hashed
                Role = userVM.Role,
                PhoneNo = userVM.PhoneNo,
                City = userVM.City,
                ProfileImagePath = profileImagePath
            };
            _userRepository.Add(user);
            _userRepository.Save();
            return Ok( new { message = "User registered successfully" });
        }
        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            // Verify hashed password
            var hashedPassword = HashPassword(password);
            if (user.Password != hashedPassword)
            {
                return Unauthorized("Invalid credentials");
            }
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, Role = user.Role, UserId = user.UserId });
        }
    }
}