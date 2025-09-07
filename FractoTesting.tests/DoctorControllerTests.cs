using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FractoBackend.Controllers;
using FractoBackend.Models;
using System.Collections.Generic;
using System.Linq;
public class DoctorControllerTests
{
    private readonly Mock<IDoctorRepository> _mockDoctorRepo;
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
    private readonly DoctorController _controller;
    public DoctorControllerTests()
    {
        _mockDoctorRepo = new Mock<IDoctorRepository>();
        _mockAppointmentRepo = new Mock<IAppointmentRepository>();
        _controller = new DoctorController(_mockDoctorRepo.Object, _mockAppointmentRepo.Object);
    }
    [Fact]
    public void GetAllDoctors_ReturnsOkWithDoctors()
    {
        _mockDoctorRepo.Setup(r => r.GetAll()).Returns(new List<Doctor> { new Doctor { DoctorId = 1, Name = "Doc1" } });
        var result = _controller.GetAllDoctors() as OkObjectResult;
        Assert.NotNull(result);
        var doctors = Assert.IsType<List<Doctor>>(result.Value);
        Assert.Single(doctors);
    }
    [Fact]
    public void GetDoctorById_Exists_ReturnsOk()
    {
        _mockDoctorRepo.Setup(r => r.GetById(1)).Returns(new Doctor { DoctorId = 1 });
        var result = _controller.GetDoctorById(1) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<Doctor>(result.Value);
    }
    [Fact]
    public void GetDoctorById_NotFound_ReturnsNotFound()
    {
        _mockDoctorRepo.Setup(r => r.GetById(1)).Returns((Doctor)null);
        var result = _controller.GetDoctorById(1);
        Assert.IsType<NotFoundObjectResult>(result);
    }
    [Fact]
    public void GetDoctorsBySpecializationAndCity_ReturnsFilteredDoctors()
    {
        var docs = new List<Doctor>
        {
            new Doctor { DoctorId = 1, SpecializationId = 1, City = "Pune" },
            new Doctor { DoctorId = 2, SpecializationId = 2, City = "Mumbai" }
        };
        _mockDoctorRepo.Setup(r => r.GetAll()).Returns(docs);
        var result = _controller.GetDoctorsBySpecializationAndCity(1, "Pune") as OkObjectResult;
        var doctors = Assert.IsType<List<Doctor>>(result.Value);
        Assert.Single(doctors);
    }
    [Fact]
    public void GetAvailableDoctors_ReturnsOk()
    {
        _mockDoctorRepo.Setup(r => r.GetAvailableDoctors(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateOnly>()))
            .Returns(new List<Doctor> { new Doctor { DoctorId = 1 } });
        var result = _controller.GetAvailableDoctors(1, "Pune", DateOnly.FromDateTime(DateTime.Today)) as OkObjectResult;
        var doctors = Assert.IsType<List<Doctor>>(result.Value);
        Assert.Single(doctors);
    }
    [Fact]
    public void GetAvailableTimeSlots_ReturnsAvailableSlots()
    {
        _mockAppointmentRepo.Setup(r => r.GetBookedTimeSlots(1, It.IsAny<DateOnly>()))
            .Returns(new List<string> { "09:00 AM" });
        var result = _controller.GetAvailableTimeSlots(1, DateOnly.FromDateTime(DateTime.Today)) as OkObjectResult;
        var slots = Assert.IsType<List<string>>(result.Value);
        Assert.DoesNotContain("09:00 AM", slots);
    }
    [Fact]
    public void GetTopRatedDoctors_ReturnsTopDoctors()
    {
        var docs = new List<Doctor>
        {
            new Doctor { DoctorId = 1, Rating = 5 },
            new Doctor { DoctorId = 2, Rating = 3 }
        };
        _mockDoctorRepo.Setup(r => r.GetAll()).Returns(docs);
        var result = _controller.GetTopRatedDoctors() as OkObjectResult;
        var doctors = Assert.IsType<List<Doctor>>(result.Value);
        Assert.Equal(2, doctors.Count);
        Assert.Equal(5, doctors.First().Rating);
    }
    [Fact]
    public void AddDoctor_ReturnsOk()
    {
        var vm = new DoctorVM { Name = "Doc", City = "Pune", SpecializationId = 1, Rating = 4 };
        var result = _controller.AddDoctor(vm) as OkObjectResult;
        Assert.Equal("Doctor added successfully", result.Value);
    }
    [Fact]
    public void UpdateDoctor_DoctorExists_ReturnsOk()
    {
        _mockDoctorRepo.Setup(r => r.GetById(1)).Returns(new Doctor { DoctorId = 1 });
        var vm = new DoctorVM { Name = "UpdatedDoc", City = "Mumbai", SpecializationId = 1, Rating = 5 };
        var result = _controller.UpdateDoctor(1, vm) as OkObjectResult;
        Assert.Equal("Doctor updated successfully", result.Value);
    }
    [Fact]
    public void UpdateDoctor_NotFound_ReturnsNotFound()
    {
        _mockDoctorRepo.Setup(r => r.GetById(1)).Returns((Doctor)null);
        var result = _controller.UpdateDoctor(1, new DoctorVM());
        Assert.IsType<NotFoundObjectResult>(result);
    }
    [Fact]
    public void DeleteDoctor_Exists_ReturnsOk()
    {
        _mockDoctorRepo.Setup(r => r.GetById(1)).Returns(new Doctor { DoctorId = 1 });
        var result = _controller.DeleteDoctor(1) as OkObjectResult;
        Assert.Equal("Doctor deleted successfully", result.Value);
    }
    [Fact]
    public void DeleteDoctor_NotFound_ReturnsNotFound()
    {
        _mockDoctorRepo.Setup(r => r.GetById(1)).Returns((Doctor)null);
        var result = _controller.DeleteDoctor(1);
        Assert.IsType<NotFoundObjectResult>(result);
    }
}


