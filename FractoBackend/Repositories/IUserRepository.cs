using FractoBackend.Models;
public interface IUserRepository
{
    User GetById(int id);
    User GetByUsername(string username);
    IEnumerable<User> GetAll();
    IEnumerable<User> GetByCity(string city);
    void Add(User user);
    void Update(User user);
    void Delete(int id);
    void Save();
}