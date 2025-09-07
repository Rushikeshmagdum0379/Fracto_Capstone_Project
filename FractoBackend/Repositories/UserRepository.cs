using FractoBackend.Models;
public class UserRepository : IUserRepository
{
    private readonly FractoDbContext _context;
    
    // Dependancy Injection
    public UserRepository(FractoDbContext context)
    {
        _context = context;
    }
    public User GetById(int id)
    {
        return _context.Users.Find(id);
    }
    public User GetByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }
    public IEnumerable<User> GetAll()
    {
        return _context.Users.ToList();
    }
    public IEnumerable<User> GetByCity(string city)
    {
        return _context.Users.Where(u => u.City == city).ToList();
    }
    public void Add(User user)
    {
        _context.Users.Add(user);
    }
    public void Update(User user)
    {
        _context.Users.Update(user);
    }
    public void Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }
    public void Save()
    {
        _context.SaveChanges();
    }
}