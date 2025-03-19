CREATE DATABASE ClubManagement;
GO
USE ClubManagement;
GO

CREATE TABLE Clubs (
    ClubID INT PRIMARY KEY IDENTITY(1,1),
    ClubName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EstablishedDate DATE NULL
);

-- (cải tiến: thêm StudentID, JoinDate, Status)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    StudentID NVARCHAR(10) NULL UNIQUE, -- Mã sinh viên FPT
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL, -- Nên mã hóa trong thực tế
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'President', 'VicePresident', 'TeamLeader', 'Member')),
    ClubID INT NULL,
    JoinDate DATE DEFAULT GETDATE(),
    Status BIT DEFAULT 1, -- 1: Active, 0: Inactive
    FOREIGN KEY (ClubID) REFERENCES Clubs(ClubID)
);

-- (cải tiến: thêm Capacity, Status)
CREATE TABLE Events (
    EventID INT PRIMARY KEY IDENTITY(1,1),
    EventName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EventDate DATETIME NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    ClubID INT NOT NULL,
    Capacity INT NULL, -- Sức chứa tối đa của sự kiện
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Upcoming', 'Ongoing', 'Completed', 'Cancelled')) DEFAULT 'Upcoming',
    FOREIGN KEY (ClubID) REFERENCES Clubs(ClubID)
);

-- (cải tiến: thêm RegistrationDate, UNIQUE constraint)
CREATE TABLE EventParticipants (
    EventParticipantID INT PRIMARY KEY IDENTITY(1,1),
    EventID INT NOT NULL,
    UserID INT NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Registered', 'Attended', 'Absent')),
    RegistrationDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EventID) REFERENCES Events(EventID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT UQ_Event_User UNIQUE (EventID, UserID) -- Ngăn đăng ký trùng
);

-- (cải tiến: CHECK constraint)
CREATE TABLE Semesters (
    SemesterID INT PRIMARY KEY IDENTITY(1,1),
    SemesterName NVARCHAR(20) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    CONSTRAINT CHK_Semester_Dates CHECK (StartDate < EndDate)
);

-- (cải tiến: liên kết Semesters, thêm ReportStatus)
CREATE TABLE Reports (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    ClubID INT NOT NULL,
    SemesterID INT NOT NULL,
    MemberChanges NVARCHAR(MAX) NULL,
    EventSummary NVARCHAR(MAX) NULL,
    ParticipationStats NVARCHAR(MAX) NULL,
    ReportStatus NVARCHAR(20) NOT NULL CHECK (ReportStatus IN ('Draft', 'Submitted', 'Approved')) DEFAULT 'Draft',
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ClubID) REFERENCES Clubs(ClubID),
    FOREIGN KEY (SemesterID) REFERENCES Semesters(SemesterID)
);

-- (cải tiến: thêm TeamLeaderID)
CREATE TABLE Teams (
    TeamID INT PRIMARY KEY IDENTITY(1,1),
    TeamName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ClubID INT NOT NULL,
    TeamLeaderID INT NULL, -- Trưởng nhóm (optional)
    FOREIGN KEY (ClubID) REFERENCES Clubs(ClubID),
    FOREIGN KEY (TeamLeaderID) REFERENCES Users(UserID)
);

CREATE TABLE TeamMembers (
    TeamMemberID INT PRIMARY KEY IDENTITY(1,1),
    TeamID INT NOT NULL,
    UserID INT NOT NULL,
    JoinDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);