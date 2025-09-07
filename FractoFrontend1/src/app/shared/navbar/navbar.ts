import { Component } from '@angular/core';
import { AuthService } from '../../services/auth';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-navbar',
  imports: [CommonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar {
  name: string = '';
  constructor(private authService: AuthService) {}
  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.name = this.authService.getUserName();
      console.log("Navbar - User is logged in as:", this.name);
    }
  }
  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }
  getRole(): string | null {
    return this.authService.getRole();
  }
  redirectToDashboard(): void {
    const role = this.getRole();
    if (role) {
      this.authService.redirectBasedOnRole(role);
    }
  }
  logout(): void {
    this.authService.logout();
    // Redirect to home or login page after logout
    window.location.href = '/';
  }
}











