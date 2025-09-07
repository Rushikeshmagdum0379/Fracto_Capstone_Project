using System;
using System.Collections.Generic;

namespace FractoBackend.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? PhoneNo { get; set; }

    public string? City { get; set; }

    public string? ProfileImagePath { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
