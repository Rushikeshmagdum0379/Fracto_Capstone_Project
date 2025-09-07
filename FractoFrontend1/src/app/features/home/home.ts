import { Component,OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
// import { RouterLink } from '@angular/router';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl:'./home.html',
  styleUrl: './home.css'
})


export class Home implements OnInit {
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











