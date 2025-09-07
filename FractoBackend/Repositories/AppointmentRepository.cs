using FractoBackend.Models;
using Microsoft.EntityFrameworkCore;
public class AppointmentRepository : IAppointmentRepository
{
    private readonly FractoDbContext _context;
    // Dependency Injection
    public AppointmentRepository(FractoDbContext context)
    {
        _context = context;
    }
    public Appointment GetById(int id)
    {
        return _context.Appointments.Find(id);
    }
    // New method implementation
    public IEnumerable<Appointment> GetConfirmedByUserId(int userId)
    {
        return _context.Appointments
                       .Include(a => a.Doctor)
                           .ThenInclude(d => d.Specialization)
                       .Where(a => a.UserId == userId && a.Status == "Confirmed")
                       .ToList();
    }

    public IEnumerable<Appointment> GetAll()
    {
        return _context.Appointments.ToList();
    }
    public IEnumerable<Appointment> GetByUserId(int userId)
    {
        return _context.Appointments.Where(a => a.UserId == userId).ToList();
    }
    public IEnumerable<Appointment> GetByDoctorId(int doctorId)
    {
        return _context.Appointments.Where(a => a.DoctorId == doctorId).ToList();
    }
    public IEnumerable<string> GetBookedTimeSlots(int doctorId, DateOnly date)
    {
        return _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date && a.Status == "Booked")
            .Select(a => a.TimeSlot)
            .ToList();
    }
    public void Add(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
    }
    public void Update(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
    }
    public void Delete(int id)
    {
        var appointment = _context.Appointments.Find(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
        }
    }
    public void Save()
    {
        _context.SaveChanges();
    }
}