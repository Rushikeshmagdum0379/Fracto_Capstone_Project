using System;
using System.Collections.Generic;

namespace FractoBackend.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string Name { get; set; } = null!;

    public int SpecializationId { get; set; }

    public string City { get; set; } = null!;

    public double? Rating { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual Specialization Specialization { get; set; } = null!;
}
