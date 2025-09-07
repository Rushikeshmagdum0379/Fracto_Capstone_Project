using System;
using System.Collections.Generic;

namespace FractoBackend.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public string TimeSlot { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
