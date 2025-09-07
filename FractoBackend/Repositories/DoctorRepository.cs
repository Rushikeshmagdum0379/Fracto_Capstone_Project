using FractoBackend.Models;
public class DoctorRepository : IDoctorRepository
{
    private readonly FractoDbContext _context;
    public DoctorRepository(FractoDbContext context)
    {
        _context = context;
    }
    public Doctor GetById(int id)
    {
        return _context.Doctors.Find(id);
    }
    public IEnumerable<Doctor> GetAll()
    {
        return _context.Doctors.ToList();
    }
    public IEnumerable<Doctor> GetBySpecialization(int specializationId)
    {
        return _context.Doctors.Where(d => d.SpecializationId == specializationId).ToList();
    }
    public IEnumerable<Doctor> GetByCity(string city)
    {
        return _context.Doctors.Where(d => d.City == city).ToList();
    }
    public IEnumerable<Doctor> GetByRating(double minRating)
    {
        return _context.Doctors.Where(d => d.Rating >= minRating).ToList();
    }
    public IEnumerable<Doctor> GetAvailableDoctors(int specializationId, string city, DateOnly date)
    {
        var bookedDoctorIds = _context.Appointments
            .Where(a => a.AppointmentDate == date)
            .Select(a => a.DoctorId)
            .ToList();
        return _context.Doctors
            .Where(d => d.SpecializationId == specializationId &&
                        d.City == city &&
                        !bookedDoctorIds.Contains(d.DoctorId))
            .ToList();
    }
    public void Add(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
    }
    public void Update(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
    }
   public void Delete(int id)
{
    using var transaction = _context.Database.BeginTransaction();
    try
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor == null)
            throw new ArgumentException("Doctor not found");
        // Delete related appointments first
        var appointments = _context.Appointments.Where(a => a.DoctorId == id);
        _context.Appointments.RemoveRange(appointments);
        // Delete related ratings
        var ratings = _context.Ratings.Where(r => r.DoctorId == id);
        _context.Ratings.RemoveRange(ratings);
        // Delete the doctor
        _context.Doctors.Remove(doctor);
        _context.SaveChanges();
        transaction.Commit();
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}










    public void Save()
    {
        _context.SaveChanges();
    }
}