import { Navbar } from '../navbar/navbar';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [  CommonModule,Navbar],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  role: string = '';
  constructor(private router: Router) {}
  ngOnInit(): void {
    this.role = localStorage.getItem('role') || '';
  }
  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }
}
