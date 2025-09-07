import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {
  registerForm: FormGroup;
  submitted = false;
  loading = false;
  selectedFile: File | null = null;
  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {
    // :white_tick: Form setup
    this.registerForm = this.fb.group(
      {
        username: ['', [Validators.required, Validators.minLength(3)]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
        role: ['', Validators.required],
        phoneNo: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
        city: ['', Validators.required],
        profileImage: [null]
      },
      {
        validators: this.passwordMatchValidator
      }
    );
  }
  // :white_tick: password and confirmPassword must match
  passwordMatchValidator(control: AbstractControl) {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }
  // :white_tick: Easy access for template
  get f() {
    return this.registerForm.controls;
  }
  // :white_tick: File upload
  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.registerForm.patchValue({ profileImage: file });
    }
  }
  // :white_tick: Submit form
  onSubmit() {
    this.submitted = true;
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.loading = true;
    const formValues = this.registerForm.value;
    const registerData = {
      username: formValues.username,
      password: formValues.password,
      role: formValues.role,
      phoneNo: formValues.phoneNo,
      city: formValues.city,
      profileImage: this.selectedFile
    };
    this.auth.register(registerData).subscribe({
      next: (res) => {
        this.loading = false;
        console.log(':white_tick: Registration success:', res);
        alert('User registered successfully');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.loading = false;
        console.error(':x: Registration failed:', err);
        alert('Registration failed: ' + (err.error || 'Unknown error'));
      }
    });
  }
}






