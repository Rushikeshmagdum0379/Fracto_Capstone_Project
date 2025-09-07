using Xunit;
using Moq;
using FractoBackend.Controllers;
using FractoBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
namespace FractoTesting.Tests
{
    public class AppointmentControllerTests
    {
        private readonly Mock<IAppointmentRepository> _mockRepo;
        private readonly AppointmentController _controller;
        public AppointmentControllerTests()
        {
            _mockRepo = new Mock<IAppointmentRepository>();
            _controller = new AppointmentController(_mockRepo.Object);
            // Add a fake logged-in user with id=1
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", "1"),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
        // 1. GetMyAppointments returns list
        [Fact]
        public void GetMyAppointments_ReturnsOk_WithAppointments()
        {
            var appointments = new List<Appointment> { new Appointment { AppointmentId = 1, UserId = 1 } };
            _mockRepo.Setup(r => r.GetByUserId(1)).Returns(appointments);
            var result = _controller.GetMyAppointments() as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(appointments, result.Value);
        }
        // 2. GetMyConfirmedAppointments returns confirmed
        [Fact]
        public void GetMyConfirmedAppointments_ReturnsOk()
        {
            var doctor = new Doctor { DoctorId = 1, Name = "Dr. Test", Rating = 4.5, Specialization = new Specialization { SpecializationName = "Cardio" } };
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, UserId = 1, Doctor = doctor, Status = "Confirmed", AppointmentDate = DateOnly.FromDateTime(DateTime.Today), TimeSlot = "10AM" }
            };
            _mockRepo.Setup(r => r.GetConfirmedByUserId(1)).Returns(appointments);
            var result = _controller.GetMyConfirmedAppointments() as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        // 3. BookAppointment saves new appointment
        [Fact]
        public void BookAppointment_ReturnsOk_WhenValid()
        {
            var vm = new AppointmentVM { DoctorId = 2, AppointmentDate = DateTime.Today, TimeSlot = "9AM" };
            var result = _controller.BookAppointment(vm) as OkObjectResult;
            _mockRepo.Verify(r => r.Add(It.IsAny<Appointment>()), Times.Once);
            _mockRepo.Verify(r => r.Save(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        // 4. CancelMyAppointment unauthorized if wrong user
        [Fact]
        public void CancelMyAppointment_ReturnsUnauthorized_WhenUserMismatch()
        {
            _mockRepo.Setup(r => r.GetById(5)).Returns(new Appointment { AppointmentId = 5, UserId = 2 });
            var result = _controller.CancelMyAppointment(5);
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        // 5. CancelMyAppointment cancels own appointment
        [Fact]
        public void CancelMyAppointment_ReturnsOk_WhenValid()
        {
            var appointment = new Appointment { AppointmentId = 10, UserId = 1, Status = "Booked" };
            _mockRepo.Setup(r => r.GetById(10)).Returns(appointment);
            var result = _controller.CancelMyAppointment(10) as OkObjectResult;
            _mockRepo.Verify(r => r.Update(It.IsAny<Appointment>()), Times.Once);
            _mockRepo.Verify(r => r.Save(), Times.Once);
            Assert.Equal(200, result.StatusCode);
        }
        // 6. GetAllAppointments returns all (Admin)
        [Fact]
        public void GetAllAppointments_ReturnsOk_WithList()
        {
            _mockRepo.Setup(r => r.GetAll()).Returns(new List<Appointment> { new Appointment { AppointmentId = 1 } });
            var result = _controller.GetAllAppointments() as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        // 7. ConfirmAppointment returns NotFound if missing
        [Fact]
        public void ConfirmAppointment_ReturnsNotFound_WhenMissing()
        {
            _mockRepo.Setup(r => r.GetById(99)).Returns((Appointment)null);
            var result = _controller.ConfirmAppointment(99);
            Assert.IsType<NotFoundObjectResult>(result);
        }
        // 8. ConfirmAppointment updates status
        [Fact]
        public void ConfirmAppointment_ReturnsOk_WhenValid()
        {
            var appointment = new Appointment { AppointmentId = 3, Status = "Booked" };
            _mockRepo.Setup(r => r.GetById(3)).Returns(appointment);
            var result = _controller.ConfirmAppointment(3) as OkObjectResult;
            _mockRepo.Verify(r => r.Update(appointment), Times.Once);
            _mockRepo.Verify(r => r.Save(), Times.Once);
            Assert.Equal(200, result.StatusCode);
        }
        // 9. CancelAppointmentByAdmin not found
        [Fact]
        public void CancelAppointmentByAdmin_ReturnsNotFound_WhenMissing()
        {
            _mockRepo.Setup(r => r.GetById(50)).Returns((Appointment)null);
            var result = _controller.CancelAppointmentByAdmin(50);
            Assert.IsType<NotFoundObjectResult>(result);
        }
        // 10. CancelAppointmentByAdmin cancels when valid
        [Fact]
        public void CancelAppointmentByAdmin_ReturnsOk_WhenValid()
        {
            var appointment = new Appointment { AppointmentId = 6, Status = "Booked" };
            _mockRepo.Setup(r => r.GetById(6)).Returns(appointment);
            var result = _controller.CancelAppointmentByAdmin(6) as OkObjectResult;
            _mockRepo.Verify(r => r.Update(appointment), Times.Once);
            _mockRepo.Verify(r => r.Save(), Times.Once);
            Assert.Equal(200, result.StatusCode);
        }
    }
}