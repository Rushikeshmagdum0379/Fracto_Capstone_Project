using FractoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SpecializationController : ControllerBase
    {
        private readonly ISpecializationRepository _specializationRepository;
        public SpecializationController(ISpecializationRepository specializationRepository)
        {
            _specializationRepository = specializationRepository;
        }
        // USER ENDPOINTS -------------------------------
        // GET: api/specialization/getAllSpecializations
        [HttpGet("getAllSpecializations")]
        public IActionResult GetAllSpecializations()
        {
            var specializations = _specializationRepository.GetAll();
            return Ok(specializations);
        }

        // GET: api/specialization/getSpecializationById/{id}
        [HttpGet("getSpecializationById/{id}")]
        public IActionResult GetSpecializationById(int id)
        {
            var specialization = _specializationRepository.GetById(id);
            if (specialization == null)
            {
                return NotFound("Specialization not found");
            }
            return Ok(specialization);
        }
        // ADMIN ENDPOINTS -------------------------------
        // POST: api/specialization/addSpecialization (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost("addSpecialization")]
        public IActionResult AddSpecialization([FromBody] SpecializationVM specialization)
        {
            _specializationRepository.Add(new Specialization
            {
                SpecializationName = specialization.SpecializationName
            });
            _specializationRepository.Save();
            return Ok("Specialization added successfully");
        }

        // PUT: api/specialization/updateSpecialization/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("updateSpecialization/{id}")]
        public IActionResult UpdateSpecialization(int id, [FromBody] SpecializationVM specialization)
        {
            var existing = _specializationRepository.GetById(id);
            if (existing == null)
            {
                return NotFound("Specialization not found");
            }
            existing.SpecializationName = specialization.SpecializationName;
            _specializationRepository.Update(existing);
            _specializationRepository.Save();
            return Ok("Specialization updated successfully");
        }

        // DELETE: api/specialization/deleteSpecialization/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteSpecialization/{id}")]
        public IActionResult DeleteSpecialization(int id)
        {
            var specialization = _specializationRepository.GetById(id);
            if (specialization == null)
            {
                return NotFound("Specialization not found");
            }
            _specializationRepository.Delete(id);
            _specializationRepository.Save();
            return Ok("Specialization deleted successfully");
        }
    }
}