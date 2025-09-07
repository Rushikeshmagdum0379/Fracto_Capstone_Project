using System;
using System.Collections.Generic;

namespace FractoBackend.Models;
public partial class DoctorVM
{
    // public int DoctorId { get; set; }

    public string Name { get; set; } = null!;

    public int SpecializationId { get; set; }

    public string City { get; set; } = null!;

    public double? Rating { get; set; }
}