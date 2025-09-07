using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FractoBackend.Controllers;
using FractoBackend.Models;
using System.Collections.Generic;
namespace FractoTesting.Tests
{
    public class SpecializationControllerTests
    {
        private readonly Mock<ISpecializationRepository> _mockRepo;
        private readonly SpecializationController _controller;
        public SpecializationControllerTests()
        {
            _mockRepo = new Mock<ISpecializationRepository>();
            _controller = new SpecializationController(_mockRepo.Object);
        }
        // ---------------- USER ENDPOINTS ----------------
        [Fact]
        public void GetAllSpecializations_ReturnsOk_WithSpecializations()
        {
            _mockRepo.Setup(r => r.GetAll()).Returns(new List<Specialization>
            {
                new Specialization { SpecializationId = 1, SpecializationName = "Cardiology" }
            });
            var result = _controller.GetAllSpecializations();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Specialization>>(okResult.Value);
            Assert.Single(returnValue);
        }
        [Fact]
        public void GetSpecializationById_ExistingId_ReturnsOk()
        {
            var specialization = new Specialization { SpecializationId = 1, SpecializationName = "Neurology" };
            _mockRepo.Setup(r => r.GetById(1)).Returns(specialization);
            var result = _controller.GetSpecializationById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(specialization, okResult.Value);
        }
        [Fact]
        public void GetSpecializationById_NonExistingId_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetById(1)).Returns((Specialization)null);
            var result = _controller.GetSpecializationById(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }
        // ---------------- ADMIN ENDPOINTS ----------------
        [Fact]
        public void AddSpecialization_ValidObject_ReturnsOk()
        {
            var vm = new SpecializationVM { SpecializationName = "Orthopedics" };
            var result = _controller.AddSpecialization(vm);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Specialization added successfully", okResult.Value);
            _mockRepo.Verify(r => r.Add(It.IsAny<Specialization>()), Times.Once);
            _mockRepo.Verify(r => r.Save(), Times.Once);
        }
        [Fact]
        public void UpdateSpecialization_ExistingId_ReturnsOk()
        {
            var existing = new Specialization { SpecializationId = 1, SpecializationName = "Dermatology" };
            var vm = new SpecializationVM { SpecializationName = "Updated Derm" };
            _mockRepo.Setup(r => r.GetById(1)).Returns(existing);
            var result = _controller.UpdateSpecialization(1, vm);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Specialization updated successfully", okResult.Value);
            _mockRepo.Verify(r => r.Update(existing), Times.Once);
            _mockRepo.Verify(r => r.Save(), Times.Once);
        }
        [Fact]
        public void UpdateSpecialization_NonExistingId_ReturnsNotFound()
        {
            var vm = new SpecializationVM { SpecializationName = "Urology" };
            _mockRepo.Setup(r => r.GetById(1)).Returns((Specialization)null);
            var result = _controller.UpdateSpecialization(1, vm);
            Assert.IsType<NotFoundObjectResult>(result);
        }
        [Fact]
        public void DeleteSpecialization_ExistingId_ReturnsOk()
        {
            var specialization = new Specialization { SpecializationId = 1, SpecializationName = "Gastroenterology" };
            _mockRepo.Setup(r => r.GetById(1)).Returns(specialization);
            var result = _controller.DeleteSpecialization(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Specialization deleted successfully", okResult.Value);
            _mockRepo.Verify(r => r.Delete(1), Times.Once);
            _mockRepo.Verify(r => r.Save(), Times.Once);
        }
        [Fact]
        public void DeleteSpecialization_NonExistingId_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetById(1)).Returns((Specialization)null);
            var result = _controller.DeleteSpecialization(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}



