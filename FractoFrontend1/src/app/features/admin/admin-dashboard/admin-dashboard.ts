import { Component,OnInit } from '@angular/core';
import { Dashboard } from "../../../shared/dashboard/dashboard";
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-admin-dashboard',
  imports: [Dashboard],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {
  totalUsers: number = 0;
  totalDoctors: number = 0;
  totalAppointments: number = 0;
  bookedAppointments: number = 0;
  confirmedAppointments: number = 0;
  cancelledAppointments: number = 0;
  private baseUrl = "http://localhost:5062/api"; // adjust to your API base
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    const headers = new HttpHeaders({
      "Authorization": `Bearer ${localStorage.getItem("token")}`
    });
    // Users
    //http://localhost:5062/api/User
    this.http.get<any[]>(`${this.baseUrl}/user`, { headers })
      .subscribe(data => {
       this.totalUsers = data.filter(u=>u.role !== 'Admin').length;

      });
    // Doctors
    // http://localhost:5062/api/Doctor/getAllDoctors
    this.http.get<any[]>(`${this.baseUrl}/Doctor/getAllDoctors`, { headers })
      .subscribe(data => {
        this.totalDoctors = data.length;

      });
    // Appointments
    // http://localhost:5062/api/Appointment/getAllAppointments
    this.http.get<any[]>(`${this.baseUrl}/Appointment/getAllAppointments`, { headers })
      .subscribe(data => {
        this.totalAppointments = data.length;
        this.bookedAppointments = data.filter(a => a.status === 'Booked').length;
        this.confirmedAppointments = data.filter(a => a.status === 'Confirmed').length;
        this.cancelledAppointments = data.filter(a => a.status === 'Cancelled').length;
      });
  }
}