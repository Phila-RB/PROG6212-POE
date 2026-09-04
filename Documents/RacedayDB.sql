--CREATE DATABASE RacedayDB;

--CREATE TABLE Users (
--UserID INT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
--Name VARCHAR(30) NOT NULL,
--Surname VARCHAR(30) NOT NULL,
--Access VARCHAR(30) NOT NULL CHECK (Access IN ('Organiser', 'Participant')),
--Password VARCHAR(50) NOT NULL
--);

--CREATE TABLE Category(
--CategoryID INT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
--CategoryName VARCHAR(30) NOT NULL,
--);

--CREATE TABLE Event(
--EventID INT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
--CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID),
--EventName VARCHAR(30) NOT NULL,
--Location VARCHAR(50) NOT NULL,
--Description VARCHAR(50) NOT NULL,
--Date DATE NOT NULL,
--Type VARCHAR(50) NOT NULL
--);

--CREATE TABLE Participant(
--ParticipantID INT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
--UserID INT FOREIGN KEY REFERENCES Users(UserID)
--)

--CREATE TABLE Signup(
--SignupID INT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
--EventID INT FOREIGN KEY REFERENCES Event(EventID),
--ParticipantID INT FOREIGN KEY REFERENCES Participant(ParticipantID),
--CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID)
--);

--CREATE TABLE ParticipantResults(
--PRID INT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
--ParticipantID INT FOREIGN KEY REFERENCES Participant(ParticipantID),
--EventID INT FOREIGN KEY REFERENCES Event(EventID),
--Placement INT NOT NULL,
--Time TIME NOT NULL
--);
INSERT INTO Users (Name, Surname, Access, Password) 
VALUES ('Thabo', 'Mokoena', 'Organiser', 'Thabo123'),
('Lerato', 'Dlamini', 'Organiser', 'Lerato123'),
('James', 'Nkosi', 'Participant', 'James123'),
('Amy', 'Pillay', 'Participant', 'Amy123');

