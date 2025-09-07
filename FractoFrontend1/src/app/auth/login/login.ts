import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  loginForm: FormGroup;
  submitted = false;
  errorMessage: string = '';
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }
  get f() {
    return this.loginForm.controls;
  }
  onSubmit() {
    this.submitted = true;
    if (this.loginForm.invalid) {
      return;
    }
    const { username, password } = this.loginForm.value;
    this.authService.login(username, password).subscribe({
      next: (response) => {
        // Save token and role in localStorage
        localStorage.setItem('token', response.token);
        localStorage.setItem('role', response.role);
        localStorage.setItem('username', username);
        localStorage.setItem('userId', response.userId);
        console.log('Token:', response.token);
        console.log('Role:', response.role);
        console.log('Username:', username);
        console.log('User ID:', response.userId);


        //alert on successful login
        alert('Login successful!');
        //alert ok -> redirect to dashboard based on role
        
        if (response.role === 'Admin') {
          this.router.navigate(['/admin-dashboard']);
        } else if (response.role === 'User') {
          this.router.navigate(['/user-dashboard']);
        // } else if (response.role === 'Dealer') {
        //   this.router.navigate(['/dealer-dashboard']);
        } else {
          this.router.navigate(['/home']); // fallback
        }
      },
      error: (err) => {
        this.errorMessage = 'Invalid username or password';
      }
    });
  }
}
