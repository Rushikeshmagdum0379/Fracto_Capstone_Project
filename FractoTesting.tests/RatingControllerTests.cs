using Xunit;
using Moq;
using FractoBackend.Controllers;
using FractoBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace FractoTesting.Tests
{
    public class RatingControllerTests
    {
        private readonly Mock<IRatingRepository> _mockRatingRepo;
        private readonly Mock<IDoctorRepository> _mockDoctorRepo;
        private readonly RatingController _controller;
        public RatingControllerTests()
        {
            _mockRatingRepo = new Mock<IRatingRepository>();
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _controller = new RatingController(_mockRatingRepo.Object, _mockDoctorRepo.Object);
            // Fake logged-in user with ID = 1
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", "1"),
                new Claim("role", "User")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
        // ---------------- USER TESTS ----------------
        [Fact]
        public void AddRating_ShouldReturnOk_WhenNewRating()
        {
            var ratingVM = new RatingVM { DoctorId = 1, Rating = 5 };
            _mockRatingRepo.Setup(r => r.GetByDoctorId(1)).Returns(new List<Rating>());
            var result = _controller.AddRating(ratingVM);
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public void AddRating_ShouldReturnBadRequest_WhenAlreadyRated()
        {
            var ratingVM = new RatingVM { DoctorId = 1, Rating = 4 };
            _mockRatingRepo.Setup(r => r.GetByDoctorId(1)).Returns(new List<Rating>
            {
                new Rating { DoctorId = 1, UserId = 1, Rating1 = 3 }
            });
            var result = _controller.AddRating(ratingVM);
            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("You have already rated this doctor. Use update endpoint to change rating.", badReq.Value);
        }
        [Fact]
        public void UpdateRating_ShouldReturnOk_WhenExistingRating()
        {
            var ratingVM = new RatingVM { DoctorId = 1, Rating = 5 };
            _mockRatingRepo.Setup(r => r.GetByDoctorId(1)).Returns(new List<Rating>
            {
                new Rating { DoctorId = 1, UserId = 1, Rating1 = 3 }
            });
            var result = _controller.UpdateRating(ratingVM);
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public void UpdateRating_ShouldReturnNotFound_WhenNoExistingRating()
        {
            var ratingVM = new RatingVM { DoctorId = 1, Rating = 5 };
            _mockRatingRepo.Setup(r => r.GetByDoctorId(1)).Returns(new List<Rating>());
            var result = _controller.UpdateRating(ratingVM);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No existing rating found for this doctor.", notFound.Value);
        }
        [Fact]
        public void GetMyRatings_ShouldReturnOk_WithRatings()
        {
            _mockRatingRepo.Setup(r => r.GetByUserId(1)).Returns(new List<Rating>
            {
                new Rating { RatingId = 1, DoctorId = 1, UserId = 1, Rating1 = 4 }
            });
            var result = _controller.GetMyRatings();
            var ok = Assert.IsType<OkObjectResult>(result);
            var ratings = Assert.IsAssignableFrom<IEnumerable<Rating>>(ok.Value);
            Assert.Single(ratings);
        }
        [Fact]
        public void GetRatingsForDoctor_ShouldReturnOk_WithRatings()
        {
            _mockRatingRepo.Setup(r => r.GetByDoctorId(1)).Returns(new List<Rating>
            {
                new Rating { RatingId = 1, DoctorId = 1, UserId = 2, Rating1 = 5 }
            });
            var result = _controller.GetRatingsForDoctor(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            var ratings = Assert.IsAssignableFrom<IEnumerable<Rating>>(ok.Value);
            Assert.Single(ratings);
        }
        [Fact]
        public void GetAverageRatingForDoctor_ShouldReturnOk()
        {
            _mockRatingRepo.Setup(r => r.GetAverageRatingForDoctor(1)).Returns(4.5);
            var result = _controller.GetAverageRatingForDoctor(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            var avg = ok.Value.GetType().GetProperty("AverageRating")?.GetValue(ok.Value, null);
            Assert.Equal(4.5, avg);
        }
        // ---------------- ADMIN TEST ----------------
        [Fact]
        public void GetAllRatings_ShouldReturnOk_WithAllRatings()
        {
            _mockRatingRepo.Setup(r => r.GetAll()).Returns(new List<Rating>
            {
                new Rating { RatingId = 1, DoctorId = 1, UserId = 1, Rating1 = 5 },
                new Rating { RatingId = 2, DoctorId = 2, UserId = 2, Rating1 = 3 }
            });
            var result = _controller.GetAllRatings();
            var ok = Assert.IsType<OkObjectResult>(result);
            var ratings = Assert.IsAssignableFrom<IEnumerable<Rating>>(ok.Value);
            Assert.Equal(2, ratings.Count());
        }
    }
}