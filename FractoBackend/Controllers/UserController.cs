using FractoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
namespace FractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // only Admin can manage users
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _env;
        public UserController(IUserRepository userRepository, IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _env = env;
        }
        // Helper: Hash password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        // GET: api/user
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.GetAll();
            return Ok(users);
        }
        // GET: api/user/getuser/{id}
        [HttpGet("getuser/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
        // POST: api/user/adduser (Admin adds a user)
        [HttpPost("adduser")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddUser([FromForm] UserVM userVM)
        {
            var existingUser = _userRepository.GetByUsername(userVM.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }
            string? profileImagePath = null;
            if (userVM.ProfileImage != null && userVM.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
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
            var user = new User
            {
                Username = userVM.Username,
                Password = HashPassword(userVM.Password), // hash password
                Role = userVM.Role,
                PhoneNo = userVM.PhoneNo,
                City = userVM.City,
                ProfileImagePath = profileImagePath
            };
            _userRepository.Add(user);
            _userRepository.Save();
            return Ok("User added successfully");
        }
        // PUT: api/user/update/{id} 
        [HttpPut("update/{id}")]
        public IActionResult UpdateUser(int id, [FromForm] UserVM userVM)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            user.Username = userVM.Username;
            if (!string.IsNullOrWhiteSpace(userVM.Password))
            {
                user.Password = HashPassword(userVM.Password);
            }
            user.Role = userVM.Role;
            user.PhoneNo = userVM.PhoneNo;
            user.City = userVM.City;
            // Update profile image if provided
            if (userVM.ProfileImage != null && userVM.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
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
                user.ProfileImagePath = "/uploads/" + fileName;
            }
            _userRepository.Update(user);
            _userRepository.Save();
            return Ok("User updated successfully");
        }
        
        // DELETE: api/user/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            _userRepository.Delete(id);
            _userRepository.Save();
            return Ok("User deleted successfully");
        }
    }
}