using FractoBackend.Models;
public class SpecializationRepository : ISpecializationRepository
{
    private readonly FractoDbContext _context;
    public SpecializationRepository(FractoDbContext context)
    {
        _context = context;
    }
    public Specialization GetById(int id)
    {
        return _context.Specializations.Find(id);
    }
    public IEnumerable<Specialization> GetAll()
    {
        return _context.Specializations.ToList();
    }
    public void Add(Specialization specialization)
    {
        _context.Specializations.Add(specialization);
    }
    public void Update(Specialization specialization)
    {
        _context.Specializations.Update(specialization);
    }
    public void Delete(int id)
    {
        var specialization = _context.Specializations.Find(id);
        if (specialization != null)
        {
            _context.Specializations.Remove(specialization);
        }
    }
    public void Save()
    {
        _context.SaveChanges();
    }
}