using FractoBackend.Models;
public interface IDoctorRepository
{
    Doctor GetById(int id);
    IEnumerable<Doctor> GetAll();
    IEnumerable<Doctor> GetBySpecialization(int specializationId);
    IEnumerable<Doctor> GetByCity(string city);
    IEnumerable<Doctor> GetByRating(double minRating);
    IEnumerable<Doctor> GetAvailableDoctors(int specializationId, string city, DateOnly date);
    void Add(Doctor doctor);
    void Update(Doctor doctor);
    void Delete(int id);
    void Save();
}