using FractoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // only logged in users can access doctors
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        public DoctorController(IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
        }
        // GET: api/doctor/getAllDoctors
        [HttpGet("getAllDoctors")]
        public IActionResult GetAllDoctors()
        {
            var doctors = _doctorRepository.GetAll();
            return Ok(doctors);
        }
        // GET: api/doctor/getDoctorById/{id}
        [HttpGet("getDoctorById/{id}")]
        public IActionResult GetDoctorById(int id)
        {
            var doctor = _doctorRepository.GetById(id);
            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }
            return Ok(doctor);
        }
        // GET: api/doctor/getDoctorsBySpecializationAndCity/specialization/{specializationId}/city/{city}
        [HttpGet("getDoctorsBySpecializationAndCity/specialization/{specializationId}/city/{city}")]
        public IActionResult GetDoctorsBySpecializationAndCity(int specializationId, string city)
        {
            var doctors = _doctorRepository.GetAll()
                            .Where(d => d.SpecializationId == specializationId && d.City == city)
                            .ToList();
            return Ok(doctors);
        }
        // GET: api/doctor/availableDoctors
        [HttpGet("availableDoctors")]
        public IActionResult GetAvailableDoctors(int specializationId, string city, DateOnly date)
        {
            var doctors = _doctorRepository.GetAvailableDoctors(specializationId, city, date);
            return Ok(doctors);
        }
        // // GET: api/doctor/{id}/timeslots
        // [HttpGet("{id}/timeslots")]
        // public IActionResult GetAvailableTimeSlots(int id, DateOnly date)
        // {
        //     var bookedSlots = _appointmentRepository.GetBookedTimeSlots(id, date);
        //     var allSlots = new List<string>
        //     {
        //         "09:00 AM", "10:00 AM", "11:00 AM", "12:00 PM",
        //         "02:00 PM", "03:00 PM", "04:00 PM", "05:00 PM"
        //     };
        //     var availableSlots = allSlots.Except(bookedSlots).ToList();
        //     return Ok(availableSlots);
        // }

        //GET: api/doctor/{id}/timeslots?date=2025-09-06
        [HttpGet("{id}/timeslots")]
        public IActionResult GetAvailableTimeSlots(int id, [FromQuery] DateOnly date)
        {
            var allSlots = new List<string>
            {
                "09:00 AM", "10:00 AM", "11:00 AM", "12:00 PM",
                "02:00 PM", "03:00 PM", "04:00 PM", "05:00 PM"
            };
            var bookedSlots = _appointmentRepository.GetBookedTimeSlots(id, date);
            var availableSlots = allSlots.Except(bookedSlots).ToList();
            return Ok(availableSlots);
        }

        // GET: api/doctor/top-rated
        [HttpGet("top-rated_doctors")]
        public IActionResult GetTopRatedDoctors()
        {
            var doctors = _doctorRepository.GetAll()
                            .OrderByDescending(d => d.Rating)
                            .Take(10)
                            .ToList();
            return Ok(doctors);
        }
        // POST: api/doctor (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost("AddDoctor")]
        public IActionResult AddDoctor([FromBody] DoctorVM doctorVM)
        {
            var doctor = new Doctor
            {
                Name = doctorVM.Name,
                City = doctorVM.City,
                SpecializationId = doctorVM.SpecializationId,
                Rating = doctorVM.Rating ?? 0
            };
            _doctorRepository.Add(doctor);
            _doctorRepository.Save();
            return Ok(new { message = "Doctor added successfully", doctor });
        }
        // PUT: api/doctor/updateDoctor/{id} (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("updateDoctor/{id}")]
        public IActionResult UpdateDoctor(int id, [FromBody] DoctorVM doctorVM)
        {
            var doctor = _doctorRepository.GetById(id);
            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }
            doctor.Name = doctorVM.Name;
            doctor.City = doctorVM.City;
            doctor.SpecializationId = doctorVM.SpecializationId;
            if (doctorVM.Rating.HasValue)
                doctor.Rating = doctorVM.Rating.Value;
            _doctorRepository.Update(doctor);
            _doctorRepository.Save();
            return Ok(new { message = "Doctor updated successfully", doctor });
        }

        // DELETE: api/doctor/delete/{id} (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            try
            {
                var doctor = _doctorRepository.GetById(id);
                if (doctor == null)
                {
                    return NotFound("Doctor not found");
                }
                _doctorRepository.Delete(id);
                _doctorRepository.Save();
                return Ok(new { message = "Doctor deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error deleting doctor", error = ex.Message });
            }
            
        }
    }
}