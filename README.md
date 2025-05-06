-- Drop foreign key constraints if they exist
IF OBJECT_ID('FK_Bookings_Venues', 'F') IS NOT NULL
    ALTER TABLE Bookings DROP CONSTRAINT FK_Bookings_Venues;

IF OBJECT_ID('FK_Bookings_Events', 'F') IS NOT NULL
    ALTER TABLE Bookings DROP CONSTRAINT FK_Bookings_Events;

-- Drop tables if they exist
IF OBJECT_ID('Bookings', 'U') IS NOT NULL DROP TABLE Bookings;
IF OBJECT_ID('Events', 'U') IS NOT NULL DROP TABLE Events;
IF OBJECT_ID('Venues', 'U') IS NOT NULL DROP TABLE Venues;

-- Create Venues table
CREATE TABLE Venues (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Location NVARCHAR(500) NOT NULL,
    Capacity INT NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    ImageUrl NVARCHAR(255) NOT NULL
);

-- Create Events table
CREATE TABLE Events (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    ImageUrl NVARCHAR(255) NOT NULL
);

-- Create Bookings table
CREATE TABLE Bookings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    VenueId INT NULL,  -- Optional venue
    CustomerName NVARCHAR(255) NOT NULL,
    CustomerEmail NVARCHAR(255) NOT NULL,
    CustomerPhone NVARCHAR(255) NOT NULL,
    BookingDate DATETIME NOT NULL,
    CONSTRAINT FK_Bookings_Venues FOREIGN KEY (VenueId) REFERENCES Venues(Id),
    CONSTRAINT FK_Bookings_Events FOREIGN KEY (EventId) REFERENCES Events(Id)
);

-- Optional: Enforce VenueId nullable (again if needed)
ALTER TABLE Bookings
ALTER COLUMN VenueId INT NULL;

-- Insert sample Venues
INSERT INTO Venues (Name, Location, Capacity, Description, ImageUrl) VALUES 
('Grand Hall', '123 Main St, Cityville', 500, 'Spacious indoor venue', 'url1'),
('Open Air Theater', '456 Park Ave, Cityville', 1000, 'Outdoor performance venue', 'url2'),
('Conference Room A', '789 Business St, Cityville', 200, 'Private meeting space', 'url3');

-- Insert sample Events
INSERT INTO Events (Name, StartDate, EndDate, Description, ImageUrl) VALUES 
('Music Concert', '2025-04-15 19:00:00', '2025-04-15 22:00:00', 'Live band event', 'url1'),
('Tech Conference', '2025-05-10 09:00:00', '2025-05-10 17:00:00', 'Industry speakers & networking', 'url2'),
('Food Festival', '2025-06-20 12:00:00', '2025-06-20 20:00:00', 'Gourmet food trucks and vendors', 'url3');

-- Insert sample Bookings
INSERT INTO Bookings (CustomerName, CustomerEmail, CustomerPhone, EventId, VenueId, BookingDate) VALUES 
('Alice Johnson', 'alice@example.com', '1234567890', 1, 1, GETDATE()),
('Bob Smith', 'bob@example.com', '0987654321', 2, 3, GETDATE()),
('Charlie Brown', 'charlie@example.com', '5555555555', 3, NULL, GETDATE()); -- Example without venue

-- Recreate Views
IF OBJECT_ID('ViewVenues', 'V') IS NOT NULL DROP VIEW ViewVenues;
GO
CREATE VIEW ViewVenues AS
SELECT Id, Name, Location, Capacity FROM Venues;
GO

IF OBJECT_ID('ViewEvents', 'V') IS NOT NULL DROP VIEW ViewEvents;
GO
CREATE VIEW ViewEvents AS
SELECT e.Id, e.Name, e.StartDate, e.Description, v.Name AS Venue
FROM Events e
LEFT JOIN Venues v ON e.Id = v.Id;
GO

IF OBJECT_ID('ViewBookings', 'V') IS NOT NULL DROP VIEW ViewBookings;
GO
CREATE VIEW ViewBookings AS
SELECT b.Id, b.CustomerName, b.CustomerEmail, e.Name AS Event, b.BookingDate
FROM Bookings b
LEFT JOIN Events e ON b.EventId = e.Id;
GO
