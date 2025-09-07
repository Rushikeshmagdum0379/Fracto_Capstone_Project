import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Dashboard } from "../../../shared/dashboard/dashboard";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
interface ConfirmedAppointment {
  appointmentId: number;
  userId: number;
  doctorId: number;
  appointmentDate: string;
  timeSlot: string;
  status: string;
  doctorName: string;
  specialization: string;
  currentRating: number;
}
interface DoctorToRate {
  doctorId: number;
  doctorName: string;
  specializationName: string;
  selectedRating: number;
  alreadyRated: boolean;
  existingRatingValue?: number;
  appointments: ConfirmedAppointment[];
}
@Component({
  selector: 'app-rate-doctor',
  imports: [Dashboard, CommonModule, FormsModule],
  templateUrl: './rate-doctor.html',
  styleUrl: './rate-doctor.css'
})
export class RateDoctor implements OnInit {
  doctorsToRate: DoctorToRate[] = [];
  isLoading = false;
  private readonly apiUrl = 'http://localhost:5062/api';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.loadConfirmedAppointments();
  }
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }
  loadConfirmedAppointments(): void {
    this.isLoading = true;
    // Use the correct endpoint that returns doctor and specialization info
    this.http.get<ConfirmedAppointment[]>(`${this.apiUrl}/appointment/myConfirmed`,
      { headers: this.getAuthHeaders() })
      .subscribe({
        next: (appointments) => {
          console.log('Confirmed appointments:', appointments);
          this.processDoctorsToRate(appointments);
          this.checkExistingRatings();
        },
        error: (err) => {
          console.error('Error fetching confirmed appointments:', err);
          this.isLoading = false;
          if (err.status === 404) {
            alert('No confirmed appointments found.');
          } else {
            alert('Error loading appointments. Please try again.');
          }
        }
      });
  }
  private processDoctorsToRate(appointments: ConfirmedAppointment[]): void {
    const doctorMap = new Map<number, DoctorToRate>();
    appointments.forEach(appointment => {
      if (!doctorMap.has(appointment.doctorId)) {
        doctorMap.set(appointment.doctorId, {
          doctorId: appointment.doctorId,
          doctorName: appointment.doctorName,
          specializationName: appointment.specialization,
          selectedRating: 0,
          alreadyRated: false,
          appointments: [appointment]
        });
      } else {
        doctorMap.get(appointment.doctorId)!.appointments.push(appointment);
      }
    });
    this.doctorsToRate = Array.from(doctorMap.values());
    console.log('Doctors to rate:', this.doctorsToRate);
  }
  private checkExistingRatings(): void {
    let completedChecks = 0;
    const totalDoctors = this.doctorsToRate.length;
    if (totalDoctors === 0) {
      this.isLoading = false;
      return;
    }
    this.doctorsToRate.forEach(doctor => {
      // Check if user has already rated this doctor
      this.http.get<any[]>(`${this.apiUrl}/rating/getRatingsForDoctor/${doctor.doctorId}`,
        { headers: this.getAuthHeaders() })
        .subscribe({
          next: (ratings) => {
            // Find current user's rating for this doctor
            const userId = this.getCurrentUserId();
            const userRating = ratings.find(r => r.userId === userId);
            if (userRating) {
              doctor.alreadyRated = true;
              doctor.existingRatingValue = userRating.rating1; // Note: using Rating1 property
              doctor.selectedRating = userRating.rating1;
            }
            completedChecks++;
            if (completedChecks === totalDoctors) {
              this.isLoading = false;
            }
          },
          error: (err) => {
            console.error(`Error checking rating for doctor ${doctor.doctorId}:`, err);
            completedChecks++;
            if (completedChecks === totalDoctors) {
              this.isLoading = false;
            }
          }
        });
    });
  }
  private getCurrentUserId(): number {
    // You might need to adjust this based on how you store user ID
    const userIdStr = localStorage.getItem('userId');
    return userIdStr ? parseInt(userIdStr) : 0;
  }
  setRating(doctor: DoctorToRate, rating: number): void {
    if (!doctor.alreadyRated) {
      doctor.selectedRating = rating;
    }
  }
  submitRating(doctor: DoctorToRate): void {
    if (doctor.selectedRating === 0) {
      alert('Please select a rating before submitting.');
      return;
    }
    if (doctor.alreadyRated) {
      // Update existing rating
      // this.updateRating(doctor);
    } else {
      // Add new rating
      this.addNewRating(doctor);
    }
  }
  private addNewRating(doctor: DoctorToRate): void {
    const payload = {
      doctorId: doctor.doctorId,
      rating: doctor.selectedRating
    };
    this.http.post(`${this.apiUrl}/rating/addRating`, payload,
      { headers: this.getAuthHeaders() })
      .subscribe({
        next: (response) => {
          console.log('Rating added successfully:', response);
          doctor.alreadyRated = true;
          doctor.existingRatingValue = doctor.selectedRating;
          alert('Rating submitted successfully!');
        },
        error: (err) => {
          console.error('Error submitting rating:', err);
          if (err.status === 400 && err.error?.includes('already rated')) {
            alert('You have already rated this doctor.');
            doctor.alreadyRated = true;
          } else {
            alert('Error submitting rating. Please try again.');
          }
        }
      });
  }
  // private updateRating(doctor: DoctorToRate): void {
  //   const payload = {
  //     doctorId: doctor.doctorId,
  //     rating: doctor.selectedRating
  //   };
  //   this.http.put(`${this.apiUrl}/rating/updateRating`, payload,
  //     { headers: this.getAuthHeaders() })
  //     .subscribe({
  //       next: (response) => {
  //         console.log('Rating updated successfully:', response);
  //         doctor.existingRatingValue = doctor.selectedRating;
  //         alert('Rating updated successfully!');
  //       },
  //       error: (err) => {
  //         console.error('Error updating rating:', err);
  //         alert('Error updating rating. Please try again.');
  //       }
  //     });
  // }
  // Generate star array for template
  getStarArray(count: number = 5): number[] {
    return Array.from({length: count}, (_, i) => i + 1);
  }
  // Format date for display
  formatDate(dateString: string): string {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      });
    } catch {
      return 'Invalid Date';
    }
  }
  // Get appointment count for doctor
  getAppointmentCount(doctor: DoctorToRate): number {
    return doctor.appointments.length;
  }
}









