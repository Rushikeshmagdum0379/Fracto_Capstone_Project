# *Fracto – Project Setup & Backend Implementation*
:date: *Date:* 08/09/2025
---
## *1. Database Setup (SQL Server)*
* Created *FractoDB* database in SQL Server.
* Scaffolded models using EF Core (Database-First approach).
Generated Files:
* Models/FractoDbContext.cs
* Models/User.cs, Doctor.cs, Appointment.cs, Specialization.cs, Rating.cs
---
## *2. VS Code & Solution Setup*
bash
# Create solution
dotnet new sln -n Fracto
# Create ASP.NET Core Web API Project (Backend)
dotnet new webapi -n FractoBackend
dotnet sln Fracto.sln add FractoBackend/FractoBackend.csproj

---
## *3. Install Required NuGet Packages*
Inside FractoBackend folder:
bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore

---
## *4. Authentication (JWT)*
### Steps Implemented:
1. Added *JWT settings* in appsettings.json.
2. Configured *JWT authentication* in Program.cs.
3. Created *helper method* in AuthController to generate token.
4. Modified *Register & Login* endpoints → return JWT token after success.
---
## *5. Profile Image Upload (Registration)*
1. Created UserVM → contains registration fields + IFormFile (profile picture).
2. Modified AuthController.Register → accepts UserVM with image.
3. Saves image in wwwroot/uploads.
4. Stores image path in User.ProfileImagePath column.
:white_tick: Completed *UserController CRUD* + Profile Image Upload.
---
## *6. Controllers & Features*
### *AuthController*
* Register (with JWT + profile upload)
* Login (returns JWT)
* Logout
### *UserController*
* CRUD on users
* Profile management (/api/user/me)
* Change password
### *DoctorController*
* Filter doctors by specialization + city
* Get available doctors for a date
* Get available time slots (predefined – booked slots excluded)
* Get top-rated doctors
* Admin-only → Add/Update/Delete
### *SpecializationController*
* CRUD (Admin only)
* Get all specializations (Users)
### *RatingController*
* Users → Add/Update ratings
* Auto-update doctor’s average rating
* Admin → View all ratings
### *AppointmentController*
* Book Appointment
* Cancel My Appointment
* Admin → Confirm/Cancel/View All Appointments
* Appointment status auto-updates (Booked, Confirmed, Cancelled)
---
## *7. User Stories*
### :white_tick: Implemented
1. Authentication (Register/Login/Logout + JWT + profile upload)
2. City-based doctor selection
3. Appointment date selection
4. Specialist selection
5. View available doctors (filter: specialization + city + date)
6. Rating-based doctor filtering
7. Doctor details by ID
8. View time slots
9. Book appointment
10. Appointment confirmation
11. Cancel appointment
---
## *8. Admin Stories*
1. Admin login (via same AuthController, Role = "Admin")
2. Manage Users (CRUD)
3. Manage Appointments (View, Confirm, Cancel)
4. Send Confirmations
5. Cancel Appointments
---
## *9. Supporting Features*
* *User Profile Management:* Update profile, change password
* *Doctor Management:* Admin CRUD, auto-update average rating
* *Specializations:* CRUD for admin, list for users
* *Ratings:* Users rate doctors, admins manage/view ratings
---
## *10. Running the Project*
### *Backend*
bash
dotnet build
dotnet run

:*Backend Development Completed*

### *Frontend*
bash

--Install Angular CLI
    npm install -g @angular/cli

-- Create new angular project-
    ng new FractoFrontend
    cd FractoFrontend
-- To generate Components-

    ng g c component_name
    ng g c features/user/user-dashboard
    ex- ng g c HomeComponent
--To generate Services
    ng g s service_name

-- Routing(to redirect from one component to other component) added in app.routes.ts

-- To build and execute
    ng build
    ng serve

---
## *11. Unit Testing Setup*
bash
# Create NUnit test project
dotnet new nunit -n FractoTesting.tests
# Install testing packages
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package Moq   # optional (mocking)

### *Run Tests*
bash
dotnet build
dotnet test

---
