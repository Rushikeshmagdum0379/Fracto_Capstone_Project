// using FractoBackend.Models;
using FractoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        public AppointmentController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        // USER ENDPOINTS -------------------------------
        // GET: api/appointment/myAppointments (view own appointments)
        [HttpGet("myAppointments")]
        public IActionResult GetMyAppointments()
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var appointments = _appointmentRepository.GetByUserId(userId);
            return Ok(appointments);
        }

        // GET: api/appointment/myConfirmed
[HttpGet("myConfirmed")]
[Authorize]  // Ensure only logged-in users can access
public IActionResult GetMyConfirmedAppointments()
{
    var userId = int.Parse(User.FindFirst("id").Value);
    var appointments = _appointmentRepository.GetConfirmedByUserId(userId)
        .Select(a => new {
            a.UserId,
            a.AppointmentDate,
            a.TimeSlot,
            a.Status,
            DoctorId = a.Doctor.DoctorId,
            DoctorName = a.Doctor.Name,
            Specialization = a.Doctor.Specialization.SpecializationName,
            CurrentRating = a.Doctor.Rating
        })
        .ToList();
    return Ok(appointments);
}



        // POST: api/appointment/bookAppointment (book appointment)
        [HttpPost("bookAppointment")]
        public IActionResult BookAppointment([FromBody] AppointmentVM appointmentVM)
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var appointment = new Appointment
            {
                UserId = userId,
                DoctorId = appointmentVM.DoctorId,
                AppointmentDate = DateOnly.FromDateTime(appointmentVM.AppointmentDate),
                TimeSlot = appointmentVM.TimeSlot,
                Status = "Booked"
            };
            _appointmentRepository.Add(appointment);
            _appointmentRepository.Save();
            return Ok("Appointment booked successfully");
        }

        // PUT: api/appointment/cancelApoointment/{id} (user cancels own appointment)
        [HttpPut("cancelAppointment/{id}")]
        public IActionResult CancelMyAppointment(int id)
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null || appointment.UserId != userId)
            {
                return Unauthorized("You cannot cancel this appointment");
            }
            appointment.Status = "Cancelled";
            _appointmentRepository.Update(appointment);
            _appointmentRepository.Save();
            return Ok("Appointment cancelled successfully");
        }

        // ADMIN ENDPOINTS -------------------------------
        // GET: api/appointment/getAllAppointments (Admin view all)
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllAppointments")]
        public IActionResult GetAllAppointments()
        {
            var appointments = _appointmentRepository.GetAll();
            return Ok(appointments);
            
        }

        // PUT: api/appointment/confirmAppointment/{id} (Admin confirms booked appointment)
        [Authorize(Roles = "Admin")]
        [HttpPut("confirmAppointment/{id}")]
        public IActionResult ConfirmAppointment(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            appointment.Status = "Confirmed";
            _appointmentRepository.Update(appointment);
            _appointmentRepository.Save();
            return Ok(new { message = "Appointment confirmed succesfully", appointment });
        }
        // PUT: api/appointment/cancelAppointmentByAdmin/{id} (Admin cancels any appointment)
        [Authorize(Roles = "Admin")]
        [HttpPut("cancelAppointmentByAdmin/{id}")]
        public IActionResult CancelAppointmentByAdmin(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            appointment.Status = "Cancelled";
            _appointmentRepository.Update(appointment);
            _appointmentRepository.Save();
            return Ok("Appointment cancelled by Admin");
        }
    }
}