import { Component, OnInit } from '@angular/core';
import { Dashboard } from "../../../shared/dashboard/dashboard";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
interface Doctor {
  doctorId: number;
  name: string;
  city: string;
  rating: number;
  specializationId: number;
  specializationName?: string;
}
interface Specialization {
  specializationId: number;
  specializationName: string;
}
@Component({
  selector: 'app-manage-doctors',
  imports: [Dashboard, FormsModule, CommonModule],
  templateUrl: './manage-doctors.html',
  styleUrl: './manage-doctors.css'
})
export class ManageDoctors implements OnInit {
  doctors: Doctor[] = [];
  specializations: Specialization[] = [];
  selectedDoctor: Doctor | null = null;
  isLoading = false;

   // For Add Doctor Modal
   isAddModalOpen = false;
   newDoctor: Doctor = { doctorId: 0, name: '', city: '', rating: 0, specializationId: 0 };

  private readonly apiUrl = 'http://localhost:5062/api';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.loadData();
  }
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

   // :white_tick: Open Add Doctor Modal
   openAddModal(): void {
    this.isAddModalOpen = true;
    this.newDoctor = { doctorId: 0, name: '', city: '', rating: 0, specializationId: 0 };
  }
  // :white_tick: Close Add Doctor Modal
  closeAddModal(): void {
    this.isAddModalOpen = false;
  }
  // :white_tick: Add Doctor API call
  addDoctor(): void {
    const doctorData = {
      name: this.newDoctor.name,
      city: this.newDoctor.city,
      specializationId: this.newDoctor.specializationId,
      rating: this.newDoctor.rating || 0
    };
    this.http.post(`${this.apiUrl}/doctor/addDoctor`, doctorData, { headers: this.getAuthHeaders() })
      .subscribe({
        next: (res: any) => {
          this.doctors.push({
            ...res.doctor,
            specializationName: this.getSpecializationName(res.doctor.specializationId)
          });
          alert('Doctor added successfully!');
          this.closeAddModal();
        },
        error: (err) => {
          console.error('Error adding doctor:', err);
          alert('Error adding doctor. Please try again.');
        }
      });
  }
  // Load doctors and specializations in parallel
  loadData(): void {
    this.isLoading = true;
    const doctors$ = this.http.get<Doctor[]>(`${this.apiUrl}/doctor/getAllDoctors`,
      { headers: this.getAuthHeaders() });
    const specializations$ = this.http.get<Specialization[]>(`${this.apiUrl}/specialization/getAllSpecializations`,
      { headers: this.getAuthHeaders() });
    forkJoin({
      doctors: doctors$,
      specializations: specializations$
    }).subscribe({
      next: (result) => {
        this.specializations = result.specializations;
        this.doctors = this.mapSpecializationNames(result.doctors);
        this.isLoading = false;
        console.log('Data loaded successfully:', this.doctors);
      },
      error: (err) => {
        console.error('Error loading data:', err);
        this.isLoading = false;
        alert('Failed to load data. Please try again.');
      }
    });
  }
  // Map specialization names to doctors
  private mapSpecializationNames(doctors: Doctor[]): Doctor[] {
    return doctors.map(doctor => ({
      ...doctor,
      specializationName: this.getSpecializationName(doctor.specializationId)
    }));
  }
  // Get specialization name by ID
  private getSpecializationName(specializationId: number): string {
    const specialization = this.specializations.find(s => s.specializationId === specializationId);
    return specialization?.specializationName || 'Unknown';
  }
  // Open edit modal
  openEditModal(doctor: Doctor): void {
    this.selectedDoctor = { ...doctor };
  }
  // Close modal
  closeModal(): void {
    this.selectedDoctor = null;
  }
  // Save doctor changes
  saveDoctor(): void {
    if (!this.selectedDoctor) return;
    // Prepare update data (only send necessary fields)
    const updateData = {
      doctorId: this.selectedDoctor.doctorId,
      name: this.selectedDoctor.name,
      city: this.selectedDoctor.city,
      specializationId: this.selectedDoctor.specializationId,
      rating: this.selectedDoctor.rating
    };
    this.http.put(`${this.apiUrl}/doctor/updateDoctor/${this.selectedDoctor.doctorId}`,
      updateData, { headers: this.getAuthHeaders() })
      .subscribe({
        next: () => {
          // Update the doctor in the local array
          const index = this.doctors.findIndex(d => d.doctorId === this.selectedDoctor!.doctorId);
          if (index !== -1) {
            this.doctors[index] = {
              ...this.selectedDoctor!,
              specializationName: this.getSpecializationName(this.selectedDoctor!.specializationId)
            };
          }
          alert('Doctor updated successfully');
          this.closeModal();
        },
        error: (err) => {
          console.error('Error updating doctor:', err);
          alert('Error updating doctor. Please try again.');
        }
      });
  }
  // Delete doctor
  deleteDoctor(id: number): void {
    if (!confirm('Are you sure you want to delete this doctor? This will also delete all associated appointments.')) {
      return;
    }
    this.http.delete(`${this.apiUrl}/doctor/delete/${id}`,
      { headers: this.getAuthHeaders() })
      .subscribe({
        next: () => {
          // Remove doctor from local array
          this.doctors = this.doctors.filter(d => d.doctorId !== id);
          alert('Doctor deleted successfully!');
        },
        error: (err) => {
          console.error('Error deleting doctor:', err);
          alert('Error deleting doctor. Please try again.');
        }
      });
  }
  // Get all specializations for dropdown
  getSpecializationsList(): Specialization[] {
    return this.specializations;
  }
  // Handle specialization change in edit modal
  onSpecializationChange(event: any): void {
    if (this.selectedDoctor) {
      this.selectedDoctor.specializationId = parseInt(event.target.value);
      this.selectedDoctor.specializationName = this.getSpecializationName(this.selectedDoctor.specializationId);
    }
  }
  // Utility method to track by function for *ngFor performance
  trackByDoctorId(index: number, doctor: Doctor): number {
    return doctor.doctorId;
  }
}









