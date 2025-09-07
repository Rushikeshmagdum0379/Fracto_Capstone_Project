using FractoBackend.Models;
public class RatingRepository : IRatingRepository
{
    private readonly FractoDbContext _context;
    public RatingRepository(FractoDbContext context)
    {
        _context = context;
    }
    public Rating GetById(int id)
    {
        return _context.Ratings.Find(id);
    }
    public IEnumerable<Rating> GetAll()
    {
        return _context.Ratings.ToList();
    }
    public IEnumerable<Rating> GetByDoctorId(int doctorId)
    {
        return _context.Ratings.Where(r => r.DoctorId == doctorId).ToList();
    }
    public IEnumerable<Rating> GetByUserId(int userId)
    {
        return _context.Ratings.Where(r => r.UserId == userId).ToList();
    }
    public double GetAverageRatingForDoctor(int doctorId)
    {
        var ratings = _context.Ratings.Where(r => r.DoctorId == doctorId && r.Rating1.HasValue).ToList();
        if (ratings.Count == 0)
        {
            return 0;
        }
        return ratings.Average(r => r.Rating1.Value);
    }
    public void Add(Rating rating)
    {
        _context.Ratings.Add(rating);
    }
    public void Update(Rating rating)
    {
        _context.Ratings.Update(rating);
    }
    public void Delete(int id)
    {
        var rating = _context.Ratings.Find(id);
        if (rating != null)
        {
            _context.Ratings.Remove(rating);
        }
    }
    public void Save()
    {
        _context.SaveChanges();
    }
}