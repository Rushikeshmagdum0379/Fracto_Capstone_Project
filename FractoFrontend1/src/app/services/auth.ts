import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
export interface AuthResponse {
  token: string;
  role: string;
  userId: string;
  username?: string; // Add username to response
}
export interface RegisterRequest {
  username: string;
  password: string;
  role: string;
  phoneNo: string;
  city: string;
  profileImage?: File | null;
}
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5062/api/Auth';
  constructor(private http: HttpClient) {}
  // LOGIN -> backend expects query params
  login(username: string, password: string): Observable<AuthResponse> {
    const params = new HttpParams()
      .set('username', username)
      .set('password', password);
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, null, { params })
      .pipe(
        tap(response => {
          // Store all user data after successful login
          this.saveToken(response.token);
          localStorage.setItem('role', response.role);
          localStorage.setItem('userId', response.userId);
          localStorage.setItem('username', username); // Store the username
        })
      );
  }
  // REGISTER -> backend expects FormData
  register(data: RegisterRequest): Observable<any> {
    const formData = new FormData();
    formData.append('Username', data.username);
    formData.append('Password', data.password);
    formData.append('Role', data.role);
    formData.append('PhoneNo', data.phoneNo);
    formData.append('City', data.city);
    if (data.profileImage) {
      formData.append('ProfileImage', data.profileImage);
    }
    return this.http.post(`${this.apiUrl}/register`, formData);
  }
  // Token helpers
  saveToken(token: string) {
    localStorage.setItem('auth_token', token);
  }
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }
  getUserName(): string {
    return localStorage.getItem('username') || '';
  }
  getRole(): string | null {
    return localStorage.getItem('role');
  }
  getUserId(): string | null {
    return localStorage.getItem('userId');
  }
  redirectBasedOnRole(role: string) {
    if (role === 'Admin') {
      window.location.href = '/admin-dashboard';
    } else if (role === 'User') {
      window.location.href = '/user-dashboard';
    }
  }
  logout() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
  }
  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}








