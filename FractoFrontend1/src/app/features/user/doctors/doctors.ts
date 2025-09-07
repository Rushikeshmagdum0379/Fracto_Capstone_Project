import { Component, OnInit } from '@angular/core';
import { Dashboard } from "../../../shared/dashboard/dashboard";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';

@Component({
  selector: 'app-doctors',
  imports: [Dashboard, CommonModule, FormsModule],
  templateUrl: './doctors.html',
  styleUrl: './doctors.css'
})
export class Doctors implements OnInit {
  doctors: any[] = [];
  selectedDoctor: any = null;
  selectedDate: string = "";
  selectedSlot: string = "";
  availableSlots: string[] = [];
  errorMessage: string = "";
  today: string = "";
  
  // For searching/filtering
  filteredDoctors: any[] = [];
  searchQuery: string = '';   // Single search bar

  private apiUrl = 'http://localhost:5062/api';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.loadDoctors();
    const now = new Date();
    this.today = now.toISOString().split('T')[0]; // yyyy-MM-dd
  }
  
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  //Fetch all doctors and their specializations and ratings
  loadDoctors() {
    this.http.get<any[]>(`${this.apiUrl}/Doctor/getAllDoctors`, { headers: this.getAuthHeaders() })
      .subscribe({
        next: async (data) => {
          this.doctors = [];
          this.filteredDoctors = data;
          for (let doc of data) {
            // specialization
            try {
              const specialization: any = await this.http
                .get(`${this.apiUrl}/Specialization/getSpecializationById/${doc.specializationId}`, { headers: this.getAuthHeaders() })
                .toPromise();
                // To see console logs in browser
                console.log('Specialization API Response:', specialization);
              doc.specializationName = specialization?.specializationName || 'N/A';
            } catch {
              
              doc.specializationName = 'N/A';
            }
            // ratings
            // http://localhost:5062/api/Rating/doctor/1/average

            try {
              const rating: any = await this.http
                .get(`${this.apiUrl}/Rating/doctor/${doc.doctorId}/average`, { headers: this.getAuthHeaders() })
                .toPromise();

              // Extract the averageRating property
              doc.avgRating = rating?.averageRating || 0;
              console.log(`Doctor: ${doc.name}, AvgRating: ${doc.avgRating}`); // Debugging
              console.log(doc.avgRating);
            } catch {
              doc.avgRating = 0;
            }
            this.doctors.push(doc);
          }
        },
        error: (err) => console.error('Error loading doctors', err)
      });
  }

  //Apply single search across multiple fields(Live filtering while typing)
  applySearch(){
    const query = this.searchQuery.toLowerCase().trim();
    console.log('Search Query:', query); // Debugging
    if (!query) {
      this.filteredDoctors = this.doctors;
      console.log('Filtered Doctors (No Query):', this.filteredDoctors); // Debugging
      return;
    }
    this.filteredDoctors = this.doctors.filter((doc: any) => {
      return (
        (doc.name && doc.name.toLowerCase().includes(query)) ||
        (doc.city && doc.city.toLowerCase().includes(query)) ||
        (doc.specializationName && doc.specializationName.toLowerCase().includes(query)) ||
        (doc.rating && doc.rating.toString().includes(query)) ||
        (doc.availableDate && doc.availableDate.toLowerCase().includes(query))
      );
    });
    console.log('Filtered Doctors:', this.filteredDoctors); // Debugging
}
  

  openBookingModal(doctor: any) {
    this.selectedDoctor = doctor;
    this.selectedDate = "";
    this.selectedSlot = "";
    this.availableSlots = [];
    this.errorMessage = "";
  }
  fetchSlots() {
    if (!this.selectedDate || !this.selectedDoctor) return;
    const formattedDate = this.selectedDate; // already yyyy-MM-dd
    this.http.get<string[]>(
      `${this.apiUrl}/Doctor/${this.selectedDoctor.doctorId}/timeslots?date=${formattedDate}`,
      { headers: this.getAuthHeaders() }
    ).subscribe({
      next: (slots) => {
        this.availableSlots = slots;
        if (slots.length === 0) {
          this.errorMessage = "No slots available for this date.";
        } else {
          this.errorMessage = "";
        }
      },
      error: (err) => {
        console.error("Error fetching slots", err);
        this.errorMessage = "Could not load available slots.";
      }
    });
  }
  confirmBooking() {
    if (!this.selectedDoctor || !this.selectedDate || !this.selectedSlot) {
      alert("Please select a date and time slot.");
      return;
    }
    const bookingData = {
      doctorId: this.selectedDoctor.doctorId,
      appointmentDate: this.selectedDate,
      timeSlot: this.selectedSlot
    };
    this.http.post(`${this.apiUrl}/Appointment/bookAppointment`, bookingData, {
      headers: this.getAuthHeaders(),
      responseType: 'text'
    }).subscribe({
      next: (res) => {
        alert("Appointment booked successfully!");
        this.closeModal();
      },
      error: (err) => {
        console.error("Error booking appointment", err);
        alert("Failed to book appointment");
      }
    });
  }
  closeModal() {
    this.selectedDoctor = null;
    this.selectedDate = "";
    this.selectedSlot = "";
    this.availableSlots = [];
    this.errorMessage = "";
  }
}









