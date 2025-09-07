using FractoBackend.Models;
public interface IAppointmentRepository
{
    Appointment GetById(int id);
     IEnumerable<Appointment> GetConfirmedByUserId(int userId);
    IEnumerable<Appointment> GetAll();
    IEnumerable<Appointment> GetByUserId(int userId);
    IEnumerable<Appointment> GetByDoctorId(int doctorId);
    IEnumerable<string> GetBookedTimeSlots(int doctorId, DateOnly date);
    void Add(Appointment appointment);
    void Update(Appointment appointment);
    void Delete(int id);
    void Save();
}
