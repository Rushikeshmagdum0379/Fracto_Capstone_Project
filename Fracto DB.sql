create database FractoDB;
Use FractoDB;



-- Users (Patient) Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL, -- User or Admin
    PhoneNo NVARCHAR(15),
    City NVARCHAR(50),
    ProfileImagePath NVARCHAR(255) NULL -- To store image path
);

-- Specializations Table
CREATE TABLE Specializations (
    SpecializationId INT PRIMARY KEY IDENTITY(1,1),
    SpecializationName NVARCHAR(100) NOT NULL
);
-- Doctors Table
CREATE TABLE Doctors (
    DoctorId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    SpecializationId INT NOT NULL,
    City NVARCHAR(50) NOT NULL,
    Rating FLOAT DEFAULT 0,
    CONSTRAINT FK_Doctor_Specialization FOREIGN KEY (SpecializationId)
        REFERENCES Specializations(SpecializationId)
);
-- Appointments Table
CREATE TABLE Appointments (
    AppointmentId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    TimeSlot NVARCHAR(20) NOT NULL, -- e.g. "10:00-10:30"
    Status NVARCHAR(20) NOT NULL DEFAULT 'Booked',
    CONSTRAINT FK_Appointment_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Appointment_Doctor FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId)
);
-- Ratings Table
CREATE TABLE Ratings (
    RatingId INT PRIMARY KEY IDENTITY(1,1),
    DoctorId INT NOT NULL,
    UserId INT NOT NULL,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    CONSTRAINT FK_Rating_Doctor FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT FK_Rating_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
