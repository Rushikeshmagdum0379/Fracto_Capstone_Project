import { Component } from '@angular/core';
import { Dashboard } from "../../../shared/dashboard/dashboard";
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-manage-users',
  imports: [Dashboard,CommonModule,FormsModule],
  templateUrl: './manage-users.html',
  styleUrl: './manage-users.css'
})
export class ManageUsers implements OnInit {
  users: any[] = [];
  selectedUser: any = null;
  showModal: boolean = false;   // :white_tick: for modal visibility
  apiUrl = 'http://localhost:5062/api/user';
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.loadUsers();
  }
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }
  // :small_blue_diamond: Load all users
  loadUsers() {
    this.http.get<any[]>(this.apiUrl, { headers: this.getAuthHeaders() })
      .subscribe({
        next: (res) => {
          console.log('Fetched users:', res);
          this.users = res.filter(u => u.role !== 'Admin'); // Exclude admins
        },
        error: (err) => {
          console.error('Error fetching users:', err);
        }
      });
  }
  // :small_blue_diamond: Open edit modal
  openEditModal(user: any) {
    this.selectedUser = { ...user };
    this.showModal = true;
  }
  // :small_blue_diamond: Close modal without saving
  closeModal() {
    this.selectedUser = null;
    this.showModal = false;
  }
  // :small_blue_diamond: Save user updates
  saveUser() {
    if (!this.selectedUser) return;
    this.http.put(
      `${this.apiUrl}/update/${this.selectedUser.id}`,
      this.selectedUser,
      { headers: this.getAuthHeaders() }
    ).subscribe({
      next: () => {
        alert('User updated successfully');
        this.closeModal();
        this.loadUsers();
      },
      error: (err) => {
        console.error('Error updating user:', err);
      }
    });
  }
  // :small_blue_diamond: Delete user
  deleteUser(id: number) {
    if (!confirm('Are you sure you want to delete this user?')) return;
    // http://localhost:5062/api/User/delete/1
    this.http.delete(`${this.apiUrl}/delete/${id}`, { headers: this.getAuthHeaders() })
    .subscribe({
      next: () => {
        this.users = this.users.filter((u: any) => u.id !== id && u.userId !== id);
        alert('User deleted successfully!');
      },
      error: (err) => {
        console.error('Error deleting user', err);
        alert('Failed to delete user.');
      }
    });
  }
}










