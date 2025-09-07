import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { Home } from './features/home/home';

// Admin Components
import { AdminDashboard } from './features/admin/admin-dashboard/admin-dashboard';
import { ManageUsers } from './features/admin/manage-users/manage-users';
import { ManageDoctors } from './features/admin/manage-doctors/manage-doctors';
import { ManageAppointments } from './features/admin/manage-appointments/manage-appointments';

// User Components
import { UserDashboard } from './features/user/user-dashboard/user-dashboard';
import { Profile } from './features/user/profile/profile';
import { Bookings } from './features/user/bookings/bookings';
import { Doctors } from './features/user/doctors/doctors';
import { RateDoctor } from './features/user/rate-doctor/rate-doctor';



import { authGuard } from './guards/auth-guard';

export const routes: Routes = [

    {path : 'login', component: Login},
    {path : 'register', component: Register},
    // {path : '**', redirectTo: 'login', pathMatch: 'full' },

    {path : '', component: Home},

    {path : 'admin-dashboard', component: AdminDashboard , canActivate: [authGuard], data: { role: 'Admin' }},
    {path : 'manage-users', component: ManageUsers , canActivate: [authGuard], data: { role: 'Admin' }},
    {path : 'manage-doctors', component: ManageDoctors , canActivate: [authGuard], data: { role: 'Admin' }},
    {path : 'manage-appointments', component: ManageAppointments , canActivate: [authGuard], data: { role: 'Admin' }},

    // User Components - Paths
    {path : 'user-dashboard', component: UserDashboard , canActivate: [authGuard], data: { role: 'User' }},
    {path : 'profile', component: Profile , canActivate: [authGuard], data: { role: 'User' }},
    {path : 'bookings', component: Bookings , canActivate: [authGuard], data: { role: 'User' }},
    {path : 'doctors', component: Doctors , canActivate: [authGuard], data: { role: 'User' }},
    {path : 'rate-doctor', component: RateDoctor , canActivate: [authGuard], data: { role: 'User' }},

];

