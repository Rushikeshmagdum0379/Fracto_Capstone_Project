using System;
using System.Collections.Generic;

namespace FractoBackend.Models;

public partial class Rating
{
    public int RatingId { get; set; }

    public int DoctorId { get; set; }

    public int UserId { get; set; }

    public int? Rating1 { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
