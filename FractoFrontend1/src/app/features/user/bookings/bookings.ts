import { Component,OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Dashboard } from "../../../shared/dashboard/dashboard";

@Component({
  selector: 'app-bookings',
  imports: [CommonModule, Dashboard],
  templateUrl: './bookings.html',
  styleUrl: './bookings.css'
})
export class Bookings  implements OnInit {
  bookings: any[] = [];
  private apiUrl = 'http://localhost:5062/api';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.loadBookings();
  }
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }
  loadBookings() {
    this.http.get<any[]>(`${this.apiUrl}/Appointment/myAppointments`, { headers: this.getAuthHeaders() })
      .subscribe({
        next: (data) => {
          this.bookings = [];
          data.forEach(booking => {
            // Fetch doctor details first
            this.http.get<any>(`${this.apiUrl}/Doctor/getDoctorById/${booking.doctorId}`, { headers: this.getAuthHeaders() })
              .subscribe({
                next: (doctor) => {
                  booking.doctor = doctor;
                  // :white_tick: Correct endpoint for specialization
                  if (doctor.specializationId) {
                    this.http.get<any>(`${this.apiUrl}/specialization/getSpecializationById/${doctor.specializationId}`, { headers: this.getAuthHeaders() })
                      .subscribe({
                        next: (spec) => {
                          booking.doctor.specialization = spec.specializationName; // map correctly
                          this.bookings.push(booking);
                        },
                        error: () => {
                          booking.doctor.specialization = 'N/A';
                          this.bookings.push(booking);
                        }
                      });
                  } else {
                    booking.doctor.specialization = doctor.specialization || 'N/A';
                    this.bookings.push(booking);
                  }
                },
                error: () => {
                  booking.doctor = { name: 'Unknown', specialization: 'N/A' };
                  this.bookings.push(booking);
                }
              });
          });
        },
        error: (err) => {
          console.error('Error loading bookings', err);
        }
      });
  }
  cancelBooking(id: number) {
    if (!confirm('Are you sure you want to cancel this appointment?')) return;
    this.http.put(`${this.apiUrl}/Appointment/cancelAppointment/${id}`, {}, { headers: this.getAuthHeaders(), responseType: 'text' })
      .subscribe({
        next: () => {
          alert('Appointment cancelled successfully!');
          this.loadBookings();
        },
        error: (err) => {
          console.error('Error cancelling appointment', err);
          alert('Failed to cancel appointment');
        }
      });
  }
  get bookedAppointments() {
    return this.bookings ? this.bookings.filter(b => b.status === 'Booked') : [];
  }
  get confirmedAppointments() {
    return this.bookings ? this.bookings.filter(b => b.status === 'Confirmed') : [];
  }
  get cancelledAppointments() {
    return this.bookings ? this.bookings.filter(b => b.status === 'Cancelled') : [];
  }
  
  
  
  
}
