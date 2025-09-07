import { Component, OnInit } from '@angular/core';
import { Dashboard } from "../../../shared/dashboard/dashboard";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
interface Appointment {
  appointmentId: number;
  userId: number;
  doctorId: number;
  appointmentDate: string;
  timeSlot: string;
  status: string;
  // Enriched properties
  userName?: string;
  doctorName?: string;
  specializationName?: string;
}
interface User {
  userId: number;
  username: string;
}
interface Doctor {
  doctorId: number;
  name: string;
  specializationId: number;
  city: string;
  rating: number;
}
interface Specialization {
  specializationId: number;
  specializationName: string;
}
@Component({
  selector: 'app-manage-appointments',
  imports: [Dashboard, FormsModule, CommonModule],
  templateUrl: './manage-appointments.html',
  styleUrl: './manage-appointments.css'
})
export class ManageAppointments implements OnInit {
  allAppointments: Appointment[] = [];
  bookedAppointments: Appointment[] = [];
  confirmedAppointments: Appointment[] = [];
  cancelledAppointments: Appointment[] = [];
  users: User[] = [];
  doctors: Doctor[] = [];
  specializations: Specialization[] = [];
  isLoading = false;
  selectedTab = 'booked';
  private readonly apiUrl = 'http://localhost:5062/api';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.loadAllData();
  }
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }
  // Load all required data
  loadAllData(): void {
    this.isLoading = true;
    // First load basic lookup data
    const users$ = this.http.get<User[]>(`${this.apiUrl}/user`,
      { headers: this.getAuthHeaders() });
    const doctors$ = this.http.get<Doctor[]>(`${this.apiUrl}/doctor/getAllDoctors`,
      { headers: this.getAuthHeaders() });
    const specializations$ = this.http.get<Specialization[]>(`${this.apiUrl}/specialization/getAllSpecializations`,
      { headers: this.getAuthHeaders() });
    // Load lookup data first
    forkJoin({
      users: users$,
      doctors: doctors$,
      specializations: specializations$
    }).subscribe({
      next: (lookupData) => {
        this.users = lookupData.users;
        this.doctors = lookupData.doctors;
        this.specializations = lookupData.specializations;
        console.log('Users loaded:', this.users);
        console.log('Doctors loaded:', this.doctors);
        console.log('Specializations loaded:', this.specializations);
        // Now load appointments
        this.loadAppointments();
      },
      error: (err) => {
        console.error('Error loading lookup data:', err);
        this.isLoading = false;
        alert('Failed to load reference data. Please try again.');
      }
    });
  }
  // Load appointments after lookup data is ready
  loadAppointments(): void {
    this.http.get<Appointment[]>(`${this.apiUrl}/appointment/getAllAppointments`,
      { headers: this.getAuthHeaders() })
      .subscribe({
        next: (appointments) => {
          console.log('Raw appointments:', appointments);
          this.allAppointments = this.enrichAppointments(appointments);
          this.categorizeAppointments();
          this.isLoading = false;
          console.log('Enriched appointments:', this.allAppointments);
        },
        error: (err) => {
          console.error('Error loading appointments:', err);
          this.isLoading = false;
          alert('Failed to load appointments. Please try again.');
        }
      });
  }
  private enrichAppointments(appointments: Appointment[]): Appointment[] {
    return appointments.map(appointment => {
      const user = this.users.find(u => u.userId === appointment.userId);
      const doctor = this.doctors.find(d => d.doctorId === appointment.doctorId);
      const specialization = doctor ?
        this.specializations.find(s => s.specializationId === doctor.specializationId) : null;
      console.log(`Appointment ${appointment.appointmentId}:`, {
        userId: appointment.userId,
        doctorId: appointment.doctorId,
        foundUser: user,
        foundDoctor: doctor,
        foundSpecialization: specialization
      });
      return {
        ...appointment,
        userName: user?.username || 'Unknown User',
        doctorName: doctor?.name || 'Unknown Doctor',
        specializationName: specialization?.specializationName || 'Unknown Specialization'
      };
    });
  }
  private categorizeAppointments(): void {
    this.bookedAppointments = this.allAppointments.filter(a =>
      a.status?.toLowerCase() === 'booked');
    this.confirmedAppointments = this.allAppointments.filter(a =>
      a.status?.toLowerCase() === 'confirmed');
    this.cancelledAppointments = this.allAppointments.filter(a =>
      a.status?.toLowerCase() === 'cancelled');
    console.log('Categorized appointments:', {
      booked: this.bookedAppointments.length,
      confirmed: this.confirmedAppointments.length,
      cancelled: this.cancelledAppointments.length
    });
  }
  // Confirm a booked appointment
  confirmAppointment(appointmentId: number): void {
    if (!confirm('Are you sure you want to confirm this appointment?')) {
      return;
    }
    this.http.put(`${this.apiUrl}/appointment/confirmAppointment/${appointmentId}`, {},
      { headers: this.getAuthHeaders() })
      .subscribe({
        next: () => {
          // Update the appointment status locally
          const appointment = this.allAppointments.find(a => a.appointmentId === appointmentId);
          if (appointment) {
            appointment.status = 'Confirmed';
            this.categorizeAppointments();
          }
          alert('Appointment confirmed successfully!');
        },
        error: (err) => {
          console.error('Error confirming appointment:', err);
          alert('Error confirming appointment. Please try again.');
        }
      });
  }
  // Cancel an appointment (admin action)
  cancelAppointment(appointmentId: number): void {
    if (!confirm('Are you sure you want to cancel this appointment?')) {
      return;
    }
    this.http.put(`${this.apiUrl}/appointment/cancelAppointmentByAdmin/${appointmentId}`, {},
      { headers: this.getAuthHeaders() })
      .subscribe({
        next: () => {
          // Update the appointment status locally
          const appointment = this.allAppointments.find(a => a.appointmentId === appointmentId);
          if (appointment) {
            appointment.status = 'Cancelled';
            this.categorizeAppointments();
          }
          alert('Appointment cancelled successfully!');
        },
        error: (err) => {
          console.error('Error cancelling appointment:', err);
          alert('Error cancelling appointment. Please try again.');
        }
      });
  }
  // Switch between tabs
  switchTab(tab: string): void {
    this.selectedTab = tab;
  }
  // Get appointments for current tab
  getCurrentAppointments(): Appointment[] {
    switch (this.selectedTab) {
      case 'booked':
        return this.bookedAppointments;
      case 'confirmed':
        return this.confirmedAppointments;
      case 'cancelled':
        return this.cancelledAppointments;
      default:
        return [];
    }
  }
  // Get count for each status
  getBookedCount(): number {
    return this.bookedAppointments.length;
  }
  getConfirmedCount(): number {
    return this.confirmedAppointments.length;
  }
  getCancelledCount(): number {
    return this.cancelledAppointments.length;
  }
  // Format date for display
  formatDate(dateString: string): string {
    if (!dateString) return 'Invalid Date';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      });
    } catch (error) {
      return 'Invalid Date';
    }
  }
  // Get status badge class
  getStatusClass(status: string): string {
    if (!status) return '';
    switch (status.toLowerCase()) {
      case 'booked':
        return 'status-booked';
      case 'confirmed':
        return 'status-confirmed';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return '';
    }
  }
  // Track by function for performance
  trackByAppointmentId(index: number, appointment: Appointment): number {
    return appointment.appointmentId;
  }
  // Refresh data
  refreshData(): void {
    this.loadAllData();
  }
}














