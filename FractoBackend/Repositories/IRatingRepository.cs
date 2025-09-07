using FractoBackend.Models;
public interface IRatingRepository
{
    Rating GetById(int id);
    IEnumerable<Rating> GetAll();
    IEnumerable<Rating> GetByDoctorId(int doctorId);
    IEnumerable<Rating> GetByUserId(int userId);
    double GetAverageRatingForDoctor(int doctorId);
    void Add(Rating rating);
    void Update(Rating rating); 
    void Delete(int id);        
    void Save();
}