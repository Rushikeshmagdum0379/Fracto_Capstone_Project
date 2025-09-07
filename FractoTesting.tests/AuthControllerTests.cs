using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FractoBackend.Controllers;
using FractoBackend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
namespace FractoTesting.Tests
{
    public class AuthControllerTests : IDisposable
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly IConfiguration _configuration;
        private readonly AuthController _controller;
        // temp folder used when tests write files
        private readonly string _tempRoot;
        public AuthControllerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _envMock = new Mock<IWebHostEnvironment>();
            // Build a real IConfiguration with Jwt settings used by GenerateJwtToken
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "test_super_secret_key_which_is_long_enough"},
                {"Jwt:Issuer", "FractoBackend"},
                {"Jwt:Audience", "FractoFrontend"},
                {"Jwt:ExpiryMinutes", "60"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _controller = new AuthController(_userRepoMock.Object, _configuration, _envMock.Object);
            // unique temp folder per test class instance
            _tempRoot = Path.Combine(Path.GetTempPath(), "fracto_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempRoot);
        }
        // Helper: compute SHA256 base64 same as controller
        private static string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
            return Convert.ToBase64String(bytes);
        }
        // ----------------- REGISTER TESTS -----------------
        [Fact]
        public void Register_WhenUsernameExists_ReturnsBadRequest()
        {
            // Arrange
            var vm = new UserVM { Username = "existing", Password = "p" };
            _userRepoMock.Setup(r => r.GetByUsername("existing")).Returns(new User { Username = "existing" });
            // Act
            var result = _controller.Register(vm);
            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
        }
        [Fact]
        public void Register_NewUser_NoProfileImage_AddsAndSaves()
        {
            // Arrange
            var vm = new UserVM
            {
                Username = "newuser",
                Password = "pwd",
                Role = "User",
                City = "Pune",
                PhoneNo = "999"
            };
            _userRepoMock.Setup(r => r.GetByUsername("newuser")).Returns((User)null);
            // Act
            var result = _controller.Register(vm);
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.Save(), Times.Once);
        }
        [Fact]
        public void Register_WithProfileImage_SavesFileAndAddsUser()
        {
            // Arrange - create temp env.WebRootPath
            var testRoot = Path.Combine(_tempRoot, "wwwroot");
            _envMock.Setup(e => e.WebRootPath).Returns(testRoot);
            var content = Encoding.UTF8.GetBytes("dummy image content");
            using var ms = new MemoryStream(content);
            ms.Position = 0;
            IFormFile formFile = new FormFile(ms, 0, ms.Length, "ProfileImage", "photo.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
            var vm = new UserVM
            {
                Username = "imguser",
                Password = "pwd",
                Role = "User",
                City = "City",
                PhoneNo = "111",
                ProfileImage = formFile
            };
            _userRepoMock.Setup(r => r.GetByUsername("imguser")).Returns((User)null);
            // Act
            var result = _controller.Register(vm);
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            // check file written under wwwroot/uploads
            var uploads = Path.Combine(testRoot, "uploads");
            Assert.True(Directory.Exists(uploads), "Uploads folder should exist");
            var files = Directory.GetFiles(uploads);
            Assert.NotEmpty(files);
            _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.Save(), Times.Once);
            // cleanup files created for this test
            foreach (var f in files) File.Delete(f);
            // remove uploads dir
            if (Directory.Exists(uploads)) Directory.Delete(uploads);
        }
        [Fact]
        public void Register_ProfileImageZeroLength_DoesNotCreateUploadsFolder_ButAddsUser()
        {
            // Arrange - give env webroot but profile image length 0
            var testRoot = Path.Combine(_tempRoot, "wwwroot2");
            _envMock.Setup(e => e.WebRootPath).Returns(testRoot);
            using var ms = new MemoryStream(new byte[0]);
            ms.Position = 0;
            IFormFile formFile = new FormFile(ms, 0, 0, "ProfileImage", "empty.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
            var vm = new UserVM
            {
                Username = "emptyimg",
                Password = "pwd",
                ProfileImage = formFile
            };
            _userRepoMock.Setup(r => r.GetByUsername("emptyimg")).Returns((User)null);
            // Act
            var result = _controller.Register(vm);
            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            var uploads = Path.Combine(testRoot, "uploads");
            Assert.False(Directory.Exists(uploads), "Uploads folder should NOT exist for zero-length file");
            _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.Save(), Times.Once);
        }
        // ----------------- LOGIN TESTS -----------------
        [Fact]
        public void Login_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByUsername("nouser")).Returns((User)null);
            // Act
            var result = _controller.Login("nouser", "pass");
            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        [Fact]
        public void Login_IncorrectPassword_ReturnsUnauthorized()
        {
            // Arrange: user exists but with different password hash
            var stored = new User { UserId = 1, Username = "test", Role = "User", Password = ComputeHash("correct") };
            _userRepoMock.Setup(r => r.GetByUsername("test")).Returns(stored);
            // Act
            var result = _controller.Login("test", "wrong"); // wrong password
            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        [Fact]
        public void Login_ValidCredentials_ReturnsOkAndContainsTokenRoleUserId()
        {
            // Arrange: create user with password hash of "mypwd"
            var plain = "mypwd";
            var user = new User { UserId = 5, Username = "good", Role = "Admin", Password = ComputeHash(plain) };
            _userRepoMock.Setup(r => r.GetByUsername("good")).Returns(user);
            // Act
            var result = _controller.Login("good", plain) as OkObjectResult;
            // Assert
            Assert.NotNull(result);
            var obj = result.Value;
            // get properties via reflection (anonymous object)
            var t = obj.GetType();
            var token = t.GetProperty("Token")?.GetValue(obj) as string;
            var role = t.GetProperty("Role")?.GetValue(obj) as string;
            var userIdVal = t.GetProperty("UserId")?.GetValue(obj);
            Assert.False(string.IsNullOrEmpty(token), "Token should be present");
            Assert.Equal("Admin", role);
            Assert.Equal(user.UserId, Convert.ToInt32(userIdVal));
        }
        [Fact]
        public void Login_CaseSensitiveUsername_ReturnsUnauthorized_WhenCaseMismatch()
        {
            // Arrange: repo has "UserOne"
            var stored = new User { UserId = 2, Username = "UserOne", Password = ComputeHash("p") };
            _userRepoMock.Setup(r => r.GetByUsername("UserOne")).Returns(stored);
            // Act: try lowercase username (assuming GetByUsername is case-sensitive in your repo)
            var result = _controller.Login("userone", "p");
            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        // optional cleanup if needed after tests
        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempRoot))
                    Directory.Delete(_tempRoot, true);
            }
            catch
            {
                // ignore cleanup errors
            }
        }
    }
}