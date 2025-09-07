import { Component,OnInit } from '@angular/core';
import { Dashboard } from "../../../shared/dashboard/dashboard";
import { CommonModule } from '@angular/common';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-user-dashboard',
  imports: [Dashboard,CommonModule],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css'
})
export class UserDashboard implements OnInit {
  bookingsCount = 0;
  confirmedCount = 0;
  cancelledCount = 0;
  ratingsCount = 0;
  private apiUrl = 'http://localhost:5062/api';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.loadCounts();
  }
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');  // :point_left: make sure your login stores token in localStorage
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }
  loadCounts() {
    // --- Get User Appointments ---
    this.http.get<any[]>(`${this.apiUrl}/Appointment/myAppointments`, { headers: this.getAuthHeaders() }).subscribe({
      next: (appointments) => {
        console.log('Appointments:', appointments); // :point_left: Debug log
        this.bookingsCount = appointments.length;
        this.confirmedCount = appointments.filter(a => a.status === 'Confirmed').length;
        this.cancelledCount = appointments.filter(a => a.status === 'Cancelled').length;
      },
      error: (err) => {
        console.error('Error fetching appointments:', err);
      }
    });
    // --- Get User Ratings ---
    this.http.get<any[]>(`${this.apiUrl}/Rating/my`, { headers: this.getAuthHeaders() }).subscribe({
      next: (ratings) => {
        console.log('Ratings:', ratings); // :point_left: Debug log
        this.ratingsCount = ratings.length;
      },
      error: (err) => {
        console.error('Error fetching ratings:', err);
      }
    });
  }
}