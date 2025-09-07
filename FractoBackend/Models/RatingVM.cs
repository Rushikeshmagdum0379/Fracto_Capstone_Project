using System;
using System.Collections.Generic;

namespace FractoBackend.Models;
public class RatingVM
    {
        public int DoctorId { get; set; }
        public int Rating { get; set; } // 1 to 5
    }