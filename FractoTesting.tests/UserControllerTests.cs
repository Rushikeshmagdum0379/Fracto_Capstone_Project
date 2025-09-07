using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FractoBackend.Controllers;
using FractoBackend.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
public class UserControllerTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<IWebHostEnvironment> _mockEnv;
    private readonly UserController _controller;
    public UserControllerTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _controller = new UserController(_mockRepo.Object, _mockEnv.Object);
    }
    [Fact]
    public void GetAllUsers_ReturnsOkWithUsers()
    {
        _mockRepo.Setup(r => r.GetAll()).Returns(new List<User> { new User { UserId = 1, Username = "test" } });
        var result = _controller.GetAllUsers() as OkObjectResult;
        Assert.NotNull(result);
        var users = Assert.IsType<List<User>>(result.Value);
        Assert.Single(users);
    }
    [Fact]
    public void GetUserById_UserExists_ReturnsOk()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new User { UserId = 1 });
        var result = _controller.GetUserById(1) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<User>(result.Value);
    }
    [Fact]
    public void GetUserById_UserNotFound_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns((User)null);
        var result = _controller.GetUserById(1);
        Assert.IsType<NotFoundObjectResult>(result);
    }
    [Fact]
    public void AddUser_NewUser_ReturnsOk()
    {
        var vm = new UserVM { Username = "newuser", Password = "1234", Role = "User", City = "Pune", PhoneNo = "123" };
        _mockRepo.Setup(r => r.GetByUsername("newuser")).Returns((User)null);
        var result = _controller.AddUser(vm) as OkObjectResult;
        Assert.Equal("User added successfully", result.Value);
    }
    [Fact]
    public void AddUser_ExistingUsername_ReturnsBadRequest()
    {
        _mockRepo.Setup(r => r.GetByUsername("existing")).Returns(new User { Username = "existing" });
        var vm = new UserVM { Username = "existing", Password = "1234" };
        var result = _controller.AddUser(vm);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public void UpdateUser_UserExists_ReturnsOk()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new User { UserId = 1 });
        var vm = new UserVM { Username = "updated", Password = "pwd", Role = "User", City = "Mumbai", PhoneNo = "987" };
        var result = _controller.UpdateUser(1, vm) as OkObjectResult;
        Assert.Equal("User updated successfully", result.Value);
    }
    [Fact]
    public void UpdateUser_UserNotFound_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns((User)null);
        var result = _controller.UpdateUser(1, new UserVM());
        Assert.IsType<NotFoundObjectResult>(result);
    }
    [Fact]
    public void DeleteUser_UserExists_ReturnsOk()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new User { UserId = 1 });
        var result = _controller.DeleteUser(1) as OkObjectResult;
        Assert.Equal("User deleted successfully", result.Value);
    }
    [Fact]
    public void DeleteUser_UserNotFound_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns((User)null);
        var result = _controller.DeleteUser(1);
        Assert.IsType<NotFoundObjectResult>(result);
    }
}





