using System;
using System.Collections.Generic;

namespace FractoBackend.Models;
public class AppointmentVM
{
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; }
}