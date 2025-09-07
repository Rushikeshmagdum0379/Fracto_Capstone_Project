using FractoBackend.Models;
public interface ISpecializationRepository
{
    Specialization GetById(int id);
    IEnumerable<Specialization> GetAll();
    void Add(Specialization specialization);
    void Update(Specialization specialization);
    void Delete(int id);
    void Save();
}
