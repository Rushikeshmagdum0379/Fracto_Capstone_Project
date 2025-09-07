using FractoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IDoctorRepository _doctorRepository;
        public RatingController(IRatingRepository ratingRepository, IDoctorRepository doctorRepository)
        {
            _ratingRepository = ratingRepository;
            _doctorRepository = doctorRepository;
        }

        // USER ENDPOINTS -------------------------------
        // POST: api/rating/addRating (user adds rating for doctor)
        [HttpPost("addRating")]
        public IActionResult AddRating([FromBody] RatingVM ratingVM)
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            // Prevent duplicate ratings (1 user = 1 rating per doctor)
            var existingRatings = _ratingRepository.GetByDoctorId(ratingVM.DoctorId)
                                                   .Where(r => r.UserId == userId)
                                                   .ToList();
            if (existingRatings.Any())
            {
                return BadRequest("You have already rated this doctor. Use update endpoint to change rating.");
            }
            var rating = new Rating
            {
                DoctorId = ratingVM.DoctorId,
                UserId = userId,
                Rating1 = ratingVM.Rating
            };
            _ratingRepository.Add(rating);
            _ratingRepository.Save();
            // Update doctor's average rating 
            // (Whenever a new rating is added the average should be recalculated and stored in Doctor table)
            UpdateDoctorAverage(ratingVM.DoctorId);
            return Ok(new { message = "Rating added successfully"});
        }


        // PUT: api/rating/updateRating (user updates own rating for doctor)
        [HttpPut("updateRating")]
        public IActionResult UpdateRating([FromBody] RatingVM ratingVM)
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var existing = _ratingRepository.GetByDoctorId(ratingVM.DoctorId)
                                            .FirstOrDefault(r => r.UserId == userId);
            if (existing == null)
            {
                return NotFound("No existing rating found for this doctor.");
            }
            existing.Rating1 = ratingVM.Rating;
            _ratingRepository.Save();
           // Update doctor's average rating
            // (Whenever a existing rating is updated the average should be recalculated and stored in Doctor table)
            UpdateDoctorAverage(ratingVM.DoctorId);
            return Ok("Rating updated successfully");
        }

        // GET: api/rating/my
        [HttpGet("my")]
        public IActionResult GetMyRatings()
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var ratings = _ratingRepository.GetByUserId(userId);
            return Ok(ratings);
        }

        // GET: api/rating/getRatingsForDoctor/{doctorId}
        [HttpGet("getRatingsForDoctor/{doctorId}")]
        public IActionResult GetRatingsForDoctor(int doctorId)
        {
            var ratings = _ratingRepository.GetByDoctorId(doctorId);
            return Ok(ratings);
        }

        // GET: api/rating/doctor/{doctorId}/average
        [HttpGet("doctor/{doctorId}/average")]
        public IActionResult GetAverageRatingForDoctor(int doctorId)
        {
            var avg = _ratingRepository.GetAverageRatingForDoctor(doctorId);
            return Ok(new { DoctorId = doctorId, AverageRating = avg });
        }


        // ADMIN ENDPOINTS -------------------------------
        // GET: api/rating/getAllRatings (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllRatings")]
        public IActionResult GetAllRatings()
        {
            return Ok(_ratingRepository.GetAll());
        }

        //Helper method to update Doctor.Rating column
        //(Ensures that Doctor table always has the latest average rating)
        private void UpdateDoctorAverage(int doctorId)
        {
            var avg = _ratingRepository.GetAverageRatingForDoctor(doctorId);
            var doctor = _doctorRepository.GetById(doctorId);
            if (doctor != null)
            {
                doctor.Rating = avg;
                _doctorRepository.Update(doctor);
                _doctorRepository.Save();
            }
        }
    }
}