-- Drop the database if it exists and recreate it
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ClubManagement')
BEGIN
    ALTER DATABASE ClubManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ClubManagement;
END
GO

CREATE DATABASE ClubManagement;
GO

USE ClubManagement;
GO

-- Create all tables first
CREATE TABLE Clubs (
    ClubID INT PRIMARY KEY IDENTITY(1,1),
    ClubName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EstablishedDate DATE NULL
);
GO

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    StudentID NVARCHAR(10) NULL, -- Mã sinh viên FPT
    UserName NVARCHAR(100) NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL, -- Nên mã hóa trong thực tế
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'President', 'VicePresident', 'TeamLeader', 'Member')),
    ClubID INT NULL,
    JoinDate DATE DEFAULT GETDATE(),
    Status BIT DEFAULT 1, -- 1: Active, 0: Inactive
    FOREIGN KEY (ClubID) REFERENCES Clubs(ClubID)
);
GO

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
GO

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
GO

CREATE TABLE Semesters (
    SemesterID INT PRIMARY KEY IDENTITY(1,1),
    SemesterName NVARCHAR(20) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    CONSTRAINT CHK_Semester_Dates CHECK (StartDate < EndDate)
);
GO

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
GO

CREATE TABLE Teams (
    TeamID INT PRIMARY KEY IDENTITY(1,1),
    TeamName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ClubID INT NOT NULL,
    FOREIGN KEY (ClubID) REFERENCES Clubs(ClubID)
);
GO

CREATE TABLE TeamMembers (
    TeamMemberID INT PRIMARY KEY IDENTITY(1,1),
    TeamID INT NOT NULL,
    UserID INT NOT NULL,
    JoinDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Now insert data into tables
-- Insert data into Clubs table
INSERT INTO Clubs (ClubName, Description, EstablishedDate)
VALUES 
    ('F-Code', 'FPT University Coding Club', '2019-05-15'),
    ('Sac Mau', 'Design Club', '2019-06-20'),
    ('FPT Basketball', 'Basketball team', '2019-07-10'),
    ('Toastmasters', 'Public speaking club', '2019-08-05'),
    ('Music Club', 'For music lovers and performers', '2019-11-12'),
    ('FPT Chess Club', 'Strategic board game enthusiasts', '2019-07-15'),
    ('FPT Dance Club', 'Modern and traditional dance performances', '2020-03-10');
GO
    
-- First create admin user
INSERT INTO Users (StudentID, UserName, FullName, Email, Password, Role, ClubID, JoinDate, Status)
VALUES ('ADMIN001', 'admin', 'System Admin', 'admin@fpt.edu.vn', 'admin123', 'Admin', NULL, '2020-01-01', 1);

-- Insert club members
INSERT INTO Users (StudentID, UserName, FullName, Email, Password, Role, ClubID, JoinDate, Status)
VALUES
    -- F-Code (Club 1) - 8 members
    ('SE12345', 'user1', 'Nguyen Van A', 'anv@fpt.edu.vn', 'password123', 'President', 1, '2020-02-15', 1),
    ('SE12346', 'user2', 'Tran Thi B', 'btt@fpt.edu.vn', 'password123', 'VicePresident', 1, '2020-03-10', 1),
    ('SE12347', 'user3', 'Le Van C', 'clv@fpt.edu.vn', 'password123', 'TeamLeader', 1, '2020-04-05', 1),
    ('SE12348', 'user4', 'Pham Thi D', 'dpt@fpt.edu.vn', 'password123', 'TeamLeader', 1, '2020-04-15', 1),
    ('SE12349', 'user5', 'Hoang Van E', 'ehv@fpt.edu.vn', 'password123', 'Member', 1, '2020-05-01', 1),
    ('SE12350', 'user6', 'Nguyen Thi F', 'fnt@fpt.edu.vn', 'password123', 'Member', 1, '2020-05-15', 1),
    ('SE12351', 'user7', 'Tran Van G', 'gtv@fpt.edu.vn', 'password123', 'Member', 1, '2020-06-01', 1),
    ('SE12352', 'user8', 'Le Thi H', 'hlt@fpt.edu.vn', 'password123', 'Member', 1, '2020-06-15', 1),
    
    -- Sac Mau (Club 2) - 7 members
    ('SE22345', 'user9', 'Vo Van I', 'ivv@fpt.edu.vn', 'password123', 'President', 2, '2019-07-01', 1),
    ('SE22346', 'user10', 'Hoang Thi J', 'jht@fpt.edu.vn', 'password123', 'VicePresident', 2, '2019-07-15', 1),
    ('SE22347', 'user11', 'Nguyen Van K', 'knv@fpt.edu.vn', 'password123', 'TeamLeader', 2, '2019-08-01', 1),
    ('SE22348', 'user12', 'Tran Thi L', 'ltt@fpt.edu.vn', 'password123', 'TeamLeader', 2, '2019-08-15', 1),
    ('SE22349', 'user13', 'Le Van M', 'mlv@fpt.edu.vn', 'password123', 'Member', 2, '2019-09-01', 1),
    ('SE22350', 'user14', 'Pham Thi N', 'npt@fpt.edu.vn', 'password123', 'Member', 2, '2019-09-15', 1),
    ('SE22351', 'user15', 'Hoang Van O', 'ohv@fpt.edu.vn', 'password123', 'Member', 2, '2019-10-01', 1),
    
    -- FPT Basketball (Club 3) - 7 members
    ('SE32345', 'user16', 'Nguyen Thi P', 'pnt@fpt.edu.vn', 'password123', 'President', 3, '2019-08-01', 1),
    ('SE32346', 'user17', 'Tran Van Q', 'qtv@fpt.edu.vn', 'password123', 'VicePresident', 3, '2019-08-15', 1),
    ('SE32347', 'user18', 'Le Thi R', 'rlt@fpt.edu.vn', 'password123', 'TeamLeader', 3, '2019-09-01', 1),
    ('SE32348', 'user19', 'Pham Van S', 'spv@fpt.edu.vn', 'password123', 'TeamLeader', 3, '2019-09-15', 1),
    ('SE32349', 'user20', 'Hoang Thi T', 'tht@fpt.edu.vn', 'password123', 'Member', 3, '2019-10-01', 1),
    ('SE32350', 'user21', 'Nguyen Van U', 'unv@fpt.edu.vn', 'password123', 'Member', 3, '2019-10-15', 1),
    ('SE32351', 'user22', 'Tran Thi V', 'vtt@fpt.edu.vn', 'password123', 'Member', 3, '2019-11-01', 1),
    
    -- Toastmasters (Club 4) - 7 members
    ('SE42345', 'user23', 'Le Van W', 'wlv@fpt.edu.vn', 'password123', 'President', 4, '2019-09-01', 1),
    ('SE42346', 'user24', 'Pham Thi X', 'xpt@fpt.edu.vn', 'password123', 'VicePresident', 4, '2019-09-15', 1),
    ('SE42347', 'user25', 'Hoang Van Y', 'yhv@fpt.edu.vn', 'password123', 'TeamLeader', 4, '2019-10-01', 1),
    ('SE42348', 'user26', 'Nguyen Thi Z', 'znt@fpt.edu.vn', 'password123', 'TeamLeader', 4, '2019-10-15', 1),
    ('SE42349', 'user27', 'Tran Van AA', 'aatv@fpt.edu.vn', 'password123', 'Member', 4, '2019-11-01', 1),
    ('SE42350', 'user28', 'Le Thi BB', 'bblt@fpt.edu.vn', 'password123', 'Member', 4, '2019-11-15', 1),
    ('SE42351', 'user29', 'Pham Van CC', 'ccpv@fpt.edu.vn', 'password123', 'Member', 4, '2019-12-01', 1),
    
    -- Music Club (Club 5) - 7 members
    ('SE52345', 'user30', 'Hoang Van DD', 'ddhv@fpt.edu.vn', 'password123', 'President', 5, '2019-12-01', 1),
    ('SE52346', 'user31', 'Nguyen Thi EE', 'eent@fpt.edu.vn', 'password123', 'VicePresident', 5, '2019-12-15', 1),
    ('SE52347', 'user32', 'Tran Van FF', 'fftv@fpt.edu.vn', 'password123', 'TeamLeader', 5, '2020-01-01', 1),
    ('SE52348', 'user33', 'Le Thi GG', 'gglt@fpt.edu.vn', 'password123', 'TeamLeader', 5, '2020-01-15', 1),
    ('SE52349', 'user34', 'Pham Van HH', 'hhpv@fpt.edu.vn', 'password123', 'Member', 5, '2020-02-01', 1),
    ('SE52350', 'user35', 'Hoang Thi II', 'iiht@fpt.edu.vn', 'password123', 'Member', 5, '2020-02-15', 1),
    ('SE52351', 'user36', 'Nguyen Van JJ', 'jjnv@fpt.edu.vn', 'password123', 'Member', 5, '2020-03-01', 1),
    
    -- FPT Chess Club (Club 6) - 7 members
    ('SE62345', 'user37', 'Tran Thi KK', 'kktt@fpt.edu.vn', 'password123', 'President', 6, '2019-08-01', 1),
    ('SE62346', 'user38', 'Le Van LL', 'lllv@fpt.edu.vn', 'password123', 'VicePresident', 6, '2019-08-15', 1),
    ('SE62347', 'user39', 'Pham Thi MM', 'mmpt@fpt.edu.vn', 'password123', 'TeamLeader', 6, '2019-09-01', 1),
    ('SE62348', 'user40', 'Hoang Van NN', 'nnhv@fpt.edu.vn', 'password123', 'TeamLeader', 6, '2019-09-15', 1),
    ('SE62349', 'user41', 'Nguyen Thi OO', 'oont@fpt.edu.vn', 'password123', 'Member', 6, '2019-10-01', 1),
    ('SE62350', 'user42', 'Tran Van PP', 'pptv@fpt.edu.vn', 'password123', 'Member', 6, '2019-10-15', 1),
    ('SE62351', 'user43', 'Le Thi QQ', 'qqlt@fpt.edu.vn', 'password123', 'Member', 6, '2019-11-01', 1),
    
    -- FPT Dance Club (Club 7) - 7 members
    ('SE72345', 'user44', 'Pham Van RR', 'rrpv@fpt.edu.vn', 'password123', 'President', 7, '2020-03-15', 1),
    ('SE72346', 'user45', 'Hoang Thi SS', 'ssht@fpt.edu.vn', 'password123', 'VicePresident', 7, '2020-03-30', 1),
    ('SE72347', 'user46', 'Nguyen Van TT', 'ttnv@fpt.edu.vn', 'password123', 'TeamLeader', 7, '2020-04-15', 1),
    ('SE72348', 'user47', 'Tran Thi UU', 'uutt@fpt.edu.vn', 'password123', 'TeamLeader', 7, '2020-04-30', 1),
    ('SE72349', 'user48', 'Le Van VV', 'vvlv@fpt.edu.vn', 'password123', 'Member', 7, '2020-05-15', 1),
    ('SE72350', 'user49', 'Pham Thi WW', 'wwpt@fpt.edu.vn', 'password123', 'Member', 7, '2020-05-30', 1),
    ('SE72351', 'user50', 'Hoang Van XX', 'xxhv@fpt.edu.vn', 'password123', 'Member', 7, '2020-06-15', 1);
GO

-- Insert data into Teams table
INSERT INTO Teams (TeamName, Description, ClubID)
VALUES
    -- F-Code Teams (Club 1)
    ('Web Development', 'Web application development team', 1),
    ('Mobile Development', 'Mobile application development team', 1),
    
    -- Sac Mau Teams (Club 2)
    ('Graphic Design', 'Graphic design and visual arts team', 2),
    ('UI/UX Design', 'User interface and experience design team', 2),
    
    -- FPT Basketball Teams (Club 3)
    ('Men''s Team', 'Men''s basketball team', 3),
    ('Women''s Team', 'Women''s basketball team', 3),
    
    -- Toastmasters Teams (Club 4)
    ('Public Speaking', 'Public speaking and presentation team', 4),
    ('Debate Team', 'Competitive debate and argumentation team', 4),
    
    -- Music Club Teams (Club 5)
    ('Band', 'Live music performance band', 5),
    ('Vocal Ensemble', 'Vocal performance and choir', 5),
    
    -- FPT Chess Club Teams (Club 6)
    ('Classical Chess', 'Traditional chess team', 6),
    ('Speed Chess', 'Rapid and blitz chess team', 6),
    
    -- FPT Dance Club Teams (Club 7)
    ('Modern Dance', 'Contemporary and modern dance team', 7),
    ('Hip-Hop Dance', 'Hip-hop and street dance team', 7);
GO

-- Declare variables to store TeamIDs
DECLARE @WebDevTeamID INT;
DECLARE @MobileTeamID INT;
DECLARE @GraphicTeamID INT;
DECLARE @UIUXTeamID INT;
DECLARE @MensTeamID INT;
DECLARE @WomensTeamID INT;
DECLARE @PublicSpeakingTeamID INT;
DECLARE @DebateTeamID INT;
DECLARE @BandTeamID INT;
DECLARE @VocalTeamID INT;
DECLARE @ClassicalChessTeamID INT;
DECLARE @SpeedChessTeamID INT;
DECLARE @ModernDanceTeamID INT;
DECLARE @HipHopTeamID INT;

-- Get TeamIDs
SELECT @WebDevTeamID = TeamID FROM Teams WHERE TeamName = 'Web Development' AND ClubID = 1;
SELECT @MobileTeamID = TeamID FROM Teams WHERE TeamName = 'Mobile Development' AND ClubID = 1;
SELECT @GraphicTeamID = TeamID FROM Teams WHERE TeamName = 'Graphic Design' AND ClubID = 2;
SELECT @UIUXTeamID = TeamID FROM Teams WHERE TeamName = 'UI/UX Design' AND ClubID = 2;
SELECT @MensTeamID = TeamID FROM Teams WHERE TeamName = 'Men''s Team' AND ClubID = 3;
SELECT @WomensTeamID = TeamID FROM Teams WHERE TeamName = 'Women''s Team' AND ClubID = 3;
SELECT @PublicSpeakingTeamID = TeamID FROM Teams WHERE TeamName = 'Public Speaking' AND ClubID = 4;
SELECT @DebateTeamID = TeamID FROM Teams WHERE TeamName = 'Debate Team' AND ClubID = 4;
SELECT @BandTeamID = TeamID FROM Teams WHERE TeamName = 'Band' AND ClubID = 5;
SELECT @VocalTeamID = TeamID FROM Teams WHERE TeamName = 'Vocal Ensemble' AND ClubID = 5;
SELECT @ClassicalChessTeamID = TeamID FROM Teams WHERE TeamName = 'Classical Chess' AND ClubID = 6;
SELECT @SpeedChessTeamID = TeamID FROM Teams WHERE TeamName = 'Speed Chess' AND ClubID = 6;
SELECT @ModernDanceTeamID = TeamID FROM Teams WHERE TeamName = 'Modern Dance' AND ClubID = 7;
SELECT @HipHopTeamID = TeamID FROM Teams WHERE TeamName = 'Hip-Hop Dance' AND ClubID = 7;

-- Insert data into TeamMembers table
-- First check that none of the TeamID variables are NULL
IF @WebDevTeamID IS NOT NULL AND @MobileTeamID IS NOT NULL AND 
   @GraphicTeamID IS NOT NULL AND @UIUXTeamID IS NOT NULL AND
   @MensTeamID IS NOT NULL AND @WomensTeamID IS NOT NULL AND
   @PublicSpeakingTeamID IS NOT NULL AND @DebateTeamID IS NOT NULL AND
   @BandTeamID IS NOT NULL AND @VocalTeamID IS NOT NULL AND
   @ClassicalChessTeamID IS NOT NULL AND @SpeedChessTeamID IS NOT NULL AND
   @ModernDanceTeamID IS NOT NULL AND @HipHopTeamID IS NOT NULL
BEGIN
    INSERT INTO TeamMembers (TeamID, UserID, JoinDate)
    VALUES 
        -- Team 1: Web Development (F-Code)
        (@WebDevTeamID, 2, '2020-03-01'), -- President
        (@WebDevTeamID, 3, '2020-03-15'), -- Vice president
        (@WebDevTeamID, 4, '2020-04-10'), -- Team leader
        (@WebDevTeamID, 5, '2020-05-01'),
        (@WebDevTeamID, 6, '2020-05-15'),
        
        -- Team 2: Mobile Development (F-Code)
        (@MobileTeamID, 7, '2020-06-01'),
        (@MobileTeamID, 8, '2020-06-15'),
        (@MobileTeamID, 9, '2020-07-01'),
        
        -- Team 3: Graphic Design (Sac Mau)
        (@GraphicTeamID, 10, '2019-07-15'), -- President
        (@GraphicTeamID, 11, '2019-08-01'), -- Vice president
        (@GraphicTeamID, 12, '2019-08-15'), -- Team leader
        (@GraphicTeamID, 13, '2019-09-01'),
        
        -- Team 4: UI/UX Design (Sac Mau)
        (@UIUXTeamID, 14, '2019-09-15'),
        (@UIUXTeamID, 15, '2019-10-01'),
        (@UIUXTeamID, 16, '2019-10-15'),
        
        -- Team 5: Men's Team (FPT Basketball)
        (@MensTeamID, 17, '2019-08-15'), -- President
        (@MensTeamID, 18, '2019-09-01'), -- Vice president
        (@MensTeamID, 19, '2019-09-15'), -- Team leader
        (@MensTeamID, 20, '2019-10-01'),
        
        -- Team 6: Women's Team (FPT Basketball)
        (@WomensTeamID, 21, '2019-10-15'),
        (@WomensTeamID, 22, '2019-11-01'),
        (@WomensTeamID, 23, '2019-11-15'),
        
        -- Team 7: Public Speaking (Toastmasters)
        (@PublicSpeakingTeamID, 24, '2019-09-15'), -- President
        (@PublicSpeakingTeamID, 25, '2019-10-01'), -- Vice president
        (@PublicSpeakingTeamID, 26, '2019-10-15'), -- Team leader
        (@PublicSpeakingTeamID, 27, '2019-11-01'),
        
        -- Team 8: Debate Team (Toastmasters)
        (@DebateTeamID, 28, '2019-11-15'),
        (@DebateTeamID, 29, '2019-12-01'),
        (@DebateTeamID, 30, '2019-12-15'),
        
        -- Team 9: Band (Music Club)
        (@BandTeamID, 31, '2019-12-15'), -- President
        (@BandTeamID, 32, '2020-01-01'), -- Vice president
        (@BandTeamID, 33, '2020-01-15'), -- Team leader
        (@BandTeamID, 34, '2020-02-01'),
        
        -- Team 10: Vocal Ensemble (Music Club)
        (@VocalTeamID, 35, '2020-02-15'),
        (@VocalTeamID, 36, '2020-03-01'),
        (@VocalTeamID, 37, '2020-03-15'),
        
        -- Team 11: Classical Chess (FPT Chess Club)
        (@ClassicalChessTeamID, 38, '2019-08-15'), -- President
        (@ClassicalChessTeamID, 39, '2019-09-01'), -- Vice president
        (@ClassicalChessTeamID, 40, '2019-09-15'), -- Team leader
        (@ClassicalChessTeamID, 41, '2019-10-01'),
        
        -- Team 12: Speed Chess (FPT Chess Club)
        (@SpeedChessTeamID, 42, '2019-10-15'),
        (@SpeedChessTeamID, 43, '2019-11-01'),
        (@SpeedChessTeamID, 44, '2019-11-15'),
        
        -- Team 13: Modern Dance (FPT Dance Club)
        (@ModernDanceTeamID, 45, '2020-03-30'), -- President
        (@ModernDanceTeamID, 46, '2020-04-15'), -- Vice president
        (@ModernDanceTeamID, 47, '2020-04-30'), -- Team leader
        (@ModernDanceTeamID, 48, '2020-05-15'),
        
        -- Team 14: Hip-Hop Dance (FPT Dance Club)
        (@HipHopTeamID, 49, '2020-05-30'),
        (@HipHopTeamID, 50, '2020-06-15'),
        (@HipHopTeamID, 2, '2020-06-30');
END;
GO

-- Insert data into Events table
INSERT INTO Events (EventName, Description, EventDate, Location, ClubID, Capacity, Status)
VALUES 
    -- F-Code Events (Club 1)
    ('Annual Coding Competition', 'Annual coding competition for students', '2023-03-15', 'Alpha Building', 1, 100, 'Completed'),
    ('Web Development Workshop', 'Workshop on modern web technologies', '2023-04-20', 'Beta Building', 1, 50, 'Completed'),
    ('Mobile App Hackathon', '24-hour hackathon for mobile app development', '2023-05-25', 'Gamma Building', 1, 75, 'Completed'),
    ('Tech Talk: AI and ML', 'Discussion on artificial intelligence and machine learning', '2023-09-15', 'Alpha Building', 1, 60, 'Upcoming'),
    
    -- Sac Mau Events (Club 2)
    ('Design Exhibition', 'Annual design exhibition for student works', '2023-04-10', 'Art Gallery', 2, 120, 'Completed'),
    ('Logo Design Competition', 'Competition for designing club logos', '2023-06-05', 'Design Studio', 2, 40, 'Completed'),
    ('UI/UX Workshop', 'Workshop on user interface design principles', '2023-10-10', 'Design Lab', 2, 35, 'Upcoming'),
    
    -- FPT Basketball Events (Club 3)
    ('Inter-University Basketball Tournament', 'Basketball tournament between universities', '2023-05-20', 'Sports Complex', 3, 200, 'Completed'),
    ('Basketball Training Camp', 'Intensive basketball training for members', '2023-07-15', 'Indoor Court', 3, 50, 'Completed'),
    ('Friendly Match with HCMC University', 'Basketball match with HCMC University', '2023-11-15', 'Main Court', 3, 100, 'Upcoming'),
    
    -- Toastmasters Events (Club 4)
    ('Public Speaking Contest', 'Annual public speaking competition', '2023-04-15', 'Auditorium', 4, 150, 'Completed'),
    ('Debate Championship', 'Inter-club debate championship', '2023-06-15', 'Conference Hall', 4, 80, 'Completed'),
    ('Leadership Workshop', 'Workshop on leadership and communication', '2023-10-20', 'Meeting Room A', 4, 45, 'Upcoming'),
    
    -- Music Club Events (Club 5)
    ('Annual Concert', 'Annual music concert featuring club members', '2023-05-10', 'Main Hall', 5, 200, 'Completed'),
    ('Open Mic Night', 'Open mic night for performers', '2023-07-20', 'Coffee House', 5, 60, 'Completed'),
    ('Music Workshop', 'Workshop on music composition and performance', '2023-11-05', 'Music Room', 5, 40, 'Upcoming'),
    
    -- FPT Chess Club Events (Club 6)
    ('Chess Tournament', 'Annual chess tournament', '2023-04-25', 'Game Room', 6, 50, 'Completed'),
    ('Speed Chess Competition', 'Rapid and blitz chess competition', '2023-06-25', 'Chess Room', 6, 30, 'Completed'),
    ('Chess Strategy Workshop', 'Workshop on advanced chess strategies', '2023-10-15', 'Strategy Room', 6, 25, 'Upcoming'),
    
    -- FPT Dance Club Events (Club 7)
    ('Dance Showcase', 'Annual dance showcase', '2023-05-15', 'Performance Hall', 7, 180, 'Completed'),
    ('Dance Workshop', 'Workshop on various dance styles', '2023-07-30', 'Dance Studio', 7, 40, 'Completed'),
    ('Dance Competition', 'Inter-university dance competition', '2023-11-25', 'Main Stage', 7, 100, 'Upcoming');
GO

-- Declare variables to store EventIDs
DECLARE @CodingCompEventID INT;
DECLARE @WebDevWorkshopEventID INT;
DECLARE @MobileHackathonEventID INT;
DECLARE @TechTalkEventID INT;
DECLARE @DesignExhibitionEventID INT;
DECLARE @LogoCompetitionEventID INT;
DECLARE @UIUXWorkshopEventID INT;
DECLARE @BasketballTournamentEventID INT;
DECLARE @BasketballCampEventID INT;
DECLARE @FriendlyMatchEventID INT;
DECLARE @PublicSpeakingContestEventID INT;
DECLARE @DebateChampionshipEventID INT;
DECLARE @LeadershipWorkshopEventID INT;
DECLARE @AnnualConcertEventID INT;
DECLARE @OpenMicNightEventID INT;
DECLARE @MusicWorkshopEventID INT;
DECLARE @ChessTournamentEventID INT;
DECLARE @SpeedChessCompEventID INT;
DECLARE @ChessStrategyWorkshopEventID INT;
DECLARE @DanceShowcaseEventID INT;
DECLARE @DanceWorkshopEventID INT;
DECLARE @DanceCompetitionEventID INT;

-- Get EventIDs
SELECT @CodingCompEventID = EventID FROM Events WHERE EventName = 'Annual Coding Competition' AND ClubID = 1;
SELECT @WebDevWorkshopEventID = EventID FROM Events WHERE EventName = 'Web Development Workshop' AND ClubID = 1;
SELECT @MobileHackathonEventID = EventID FROM Events WHERE EventName = 'Mobile App Hackathon' AND ClubID = 1;
SELECT @TechTalkEventID = EventID FROM Events WHERE EventName = 'Tech Talk: AI and ML' AND ClubID = 1;
SELECT @DesignExhibitionEventID = EventID FROM Events WHERE EventName = 'Design Exhibition' AND ClubID = 2;
SELECT @LogoCompetitionEventID = EventID FROM Events WHERE EventName = 'Logo Design Competition' AND ClubID = 2;
SELECT @UIUXWorkshopEventID = EventID FROM Events WHERE EventName = 'UI/UX Workshop' AND ClubID = 2;
SELECT @BasketballTournamentEventID = EventID FROM Events WHERE EventName = 'Inter-University Basketball Tournament' AND ClubID = 3;
SELECT @BasketballCampEventID = EventID FROM Events WHERE EventName = 'Basketball Training Camp' AND ClubID = 3;
SELECT @FriendlyMatchEventID = EventID FROM Events WHERE EventName = 'Friendly Match with HCMC University' AND ClubID = 3;
SELECT @PublicSpeakingContestEventID = EventID FROM Events WHERE EventName = 'Public Speaking Contest' AND ClubID = 4;
SELECT @DebateChampionshipEventID = EventID FROM Events WHERE EventName = 'Debate Championship' AND ClubID = 4;
SELECT @LeadershipWorkshopEventID = EventID FROM Events WHERE EventName = 'Leadership Workshop' AND ClubID = 4;
SELECT @AnnualConcertEventID = EventID FROM Events WHERE EventName = 'Annual Concert' AND ClubID = 5;
SELECT @OpenMicNightEventID = EventID FROM Events WHERE EventName = 'Open Mic Night' AND ClubID = 5;
SELECT @MusicWorkshopEventID = EventID FROM Events WHERE EventName = 'Music Workshop' AND ClubID = 5;
SELECT @ChessTournamentEventID = EventID FROM Events WHERE EventName = 'Chess Tournament' AND ClubID = 6;
SELECT @SpeedChessCompEventID = EventID FROM Events WHERE EventName = 'Speed Chess Competition' AND ClubID = 6;
SELECT @ChessStrategyWorkshopEventID = EventID FROM Events WHERE EventName = 'Chess Strategy Workshop' AND ClubID = 6;
SELECT @DanceShowcaseEventID = EventID FROM Events WHERE EventName = 'Dance Showcase' AND ClubID = 7;
SELECT @DanceWorkshopEventID = EventID FROM Events WHERE EventName = 'Dance Workshop' AND ClubID = 7;
SELECT @DanceCompetitionEventID = EventID FROM Events WHERE EventName = 'Dance Competition' AND ClubID = 7;

-- Insert data into EventParticipants table
-- First check that none of the EventID variables are NULL
IF @CodingCompEventID IS NOT NULL AND @WebDevWorkshopEventID IS NOT NULL AND 
   @MobileHackathonEventID IS NOT NULL AND @TechTalkEventID IS NOT NULL AND
   @DesignExhibitionEventID IS NOT NULL AND @LogoCompetitionEventID IS NOT NULL AND
   @UIUXWorkshopEventID IS NOT NULL AND @BasketballTournamentEventID IS NOT NULL AND
   @BasketballCampEventID IS NOT NULL AND @FriendlyMatchEventID IS NOT NULL AND
   @PublicSpeakingContestEventID IS NOT NULL AND @DebateChampionshipEventID IS NOT NULL AND
   @LeadershipWorkshopEventID IS NOT NULL AND @AnnualConcertEventID IS NOT NULL AND
   @OpenMicNightEventID IS NOT NULL AND @MusicWorkshopEventID IS NOT NULL AND
   @ChessTournamentEventID IS NOT NULL AND @SpeedChessCompEventID IS NOT NULL AND
   @ChessStrategyWorkshopEventID IS NOT NULL AND @DanceShowcaseEventID IS NOT NULL AND
   @DanceWorkshopEventID IS NOT NULL AND @DanceCompetitionEventID IS NOT NULL
BEGIN
    INSERT INTO EventParticipants (EventID, UserID, Status, RegistrationDate)
    VALUES 
        -- Event 1: Annual Coding Competition (F-Code)
        (@CodingCompEventID, 2, 'Attended', '2023-03-01'),  -- Club president
        (@CodingCompEventID, 3, 'Attended', '2023-03-01'),  -- Vice president
        (@CodingCompEventID, 4, 'Attended', '2023-03-02'),  -- Team leader
        (@CodingCompEventID, 5, 'Attended', '2023-03-02'),
        (@CodingCompEventID, 6, 'Attended', '2023-03-03'),
        (@CodingCompEventID, 7, 'Attended', '2023-03-03'),
        (@CodingCompEventID, 8, 'Attended', '2023-03-04'),
        (@CodingCompEventID, 9, 'Attended', '2023-03-04'),
        
        -- Event 2: Web Development Workshop (F-Code)
        (@WebDevWorkshopEventID, 2, 'Attended', '2023-04-05'),
        (@WebDevWorkshopEventID, 3, 'Attended', '2023-04-05'),
        (@WebDevWorkshopEventID, 4, 'Attended', '2023-04-06'),
        (@WebDevWorkshopEventID, 5, 'Attended', '2023-04-06'),
        (@WebDevWorkshopEventID, 6, 'Attended', '2023-04-07'),
        (@WebDevWorkshopEventID, 10, 'Attended', '2023-04-07'),  -- Member from another club participating
        (@WebDevWorkshopEventID, 11, 'Attended', '2023-04-08'),
        
        -- Event 3: Mobile App Hackathon (F-Code)
        (@MobileHackathonEventID, 2, 'Attended', '2023-05-10'),
        (@MobileHackathonEventID, 3, 'Attended', '2023-05-10'),
        (@MobileHackathonEventID, 7, 'Attended', '2023-05-11'),
        (@MobileHackathonEventID, 8, 'Attended', '2023-05-11'),
        (@MobileHackathonEventID, 9, 'Absent', '2023-05-12'),  -- Absent member
        
        -- Event 5: Design Exhibition (Sac Mau)
        (@DesignExhibitionEventID, 10, 'Attended', '2023-04-01'),
        (@DesignExhibitionEventID, 11, 'Attended', '2023-04-01'),
        (@DesignExhibitionEventID, 12, 'Attended', '2023-04-02'),
        (@DesignExhibitionEventID, 13, 'Attended', '2023-04-02'),
        (@DesignExhibitionEventID, 14, 'Attended', '2023-04-03'),
        
        -- Event 8: Inter-University Basketball Tournament (FPT Basketball)
        (@BasketballTournamentEventID, 17, 'Attended', '2023-05-10'),
        (@BasketballTournamentEventID, 18, 'Attended', '2023-05-10'),
        (@BasketballTournamentEventID, 19, 'Attended', '2023-05-11'),
        (@BasketballTournamentEventID, 20, 'Attended', '2023-05-11'),
        (@BasketballTournamentEventID, 21, 'Attended', '2023-05-12'),
        (@BasketballTournamentEventID, 22, 'Absent', '2023-05-12'),  -- Absent member
        
        -- Event 11: Public Speaking Contest (Toastmasters)
        (@PublicSpeakingContestEventID, 24, 'Attended', '2023-04-05'),
        (@PublicSpeakingContestEventID, 25, 'Attended', '2023-04-05'),
        (@PublicSpeakingContestEventID, 26, 'Attended', '2023-04-06'),
        (@PublicSpeakingContestEventID, 27, 'Attended', '2023-04-06'),
        (@PublicSpeakingContestEventID, 28, 'Attended', '2023-04-07'),
        (@PublicSpeakingContestEventID, 29, 'Attended', '2023-04-07'),
        
        -- Event 14: Annual Concert (Music Club)
        (@AnnualConcertEventID, 31, 'Attended', '2023-05-01'),
        (@AnnualConcertEventID, 32, 'Attended', '2023-05-01'),
        (@AnnualConcertEventID, 33, 'Attended', '2023-05-02'),
        (@AnnualConcertEventID, 34, 'Attended', '2023-05-02'),
        (@AnnualConcertEventID, 35, 'Attended', '2023-05-03'),
        (@AnnualConcertEventID, 36, 'Attended', '2023-05-03'),
        (@AnnualConcertEventID, 37, 'Absent', '2023-05-04'),  -- Absent member
        
        -- Event 17: Chess Tournament (FPT Chess Club)
        (@ChessTournamentEventID, 38, 'Attended', '2023-04-15'),
        (@ChessTournamentEventID, 39, 'Attended', '2023-04-15'),
        (@ChessTournamentEventID, 40, 'Attended', '2023-04-16'),
        (@ChessTournamentEventID, 41, 'Attended', '2023-04-16'),
        (@ChessTournamentEventID, 42, 'Attended', '2023-04-17'),
        (@ChessTournamentEventID, 43, 'Attended', '2023-04-17'),
        
        -- Event 20: Dance Showcase (FPT Dance Club)
        (@DanceShowcaseEventID, 44, 'Attended', '2023-05-01'),
        (@DanceShowcaseEventID, 45, 'Attended', '2023-05-01'),
        (@DanceShowcaseEventID, 46, 'Attended', '2023-05-02'),
        (@DanceShowcaseEventID, 47, 'Attended', '2023-05-02'),
        (@DanceShowcaseEventID, 48, 'Attended', '2023-05-03'),
        (@DanceShowcaseEventID, 49, 'Attended', '2023-05-03'),
        (@DanceShowcaseEventID, 50, 'Registered', '2023-05-04');  -- Registered but not yet attended
END;
GO

-- Insert data into Semesters table
INSERT INTO Semesters (SemesterName, StartDate, EndDate)
VALUES
    ('Spring 2023', '2023-01-01', '2023-04-30'),
    ('Summer 2023', '2023-05-01', '2023-08-31'),
    ('Fall 2023', '2023-09-01', '2023-12-31'),
    ('Spring 2024', '2024-01-01', '2024-04-30');
GO

-- Insert data into Reports table
INSERT INTO Reports (ClubID, SemesterID, MemberChanges, EventSummary, ParticipationStats, ReportStatus, CreatedDate)
VALUES
    -- F-Code (Club 1) Reports
    (1, 1, 'Added 3 new members, 1 member left', 'Organized 2 successful events with high participation', 'Average participation rate: 85%', 'Approved', '2023-04-25'),
    (1, 2, 'Added 2 new members', 'Organized 1 hackathon and 1 workshop', 'Average participation rate: 78%', 'Approved', '2023-08-25'),
    (1, 3, 'No membership changes', 'Planning 2 events for the semester', 'Expected participation: 90%', 'Draft', '2023-09-15'),
    
    -- Sac Mau (Club 2) Reports
    (2, 1, 'Added 2 new members', 'Organized 1 exhibition with good feedback', 'Average participation rate: 80%', 'Approved', '2023-04-26'),
    (2, 2, 'Added 1 new member, 1 member left', 'Organized logo design competition', 'Average participation rate: 75%', 'Approved', '2023-08-26'),
    (2, 3, 'No membership changes', 'Planning UI/UX workshop', 'Expected participation: 85%', 'Draft', '2023-09-16'),
    
    -- FPT Basketball (Club 3) Reports
    (3, 1, 'No membership changes', 'Participated in inter-university tournament', 'Average participation rate: 90%', 'Approved', '2023-04-27'),
    (3, 2, 'Added 2 new members', 'Organized training camp', 'Average participation rate: 85%', 'Approved', '2023-08-27'),
    (3, 3, 'No membership changes', 'Planning friendly match', 'Expected participation: 95%', 'Draft', '2023-09-17'),
    
    -- Toastmasters (Club 4) Reports
    (4, 1, 'Added 1 new member', 'Organized public speaking contest', 'Average participation rate: 82%', 'Approved', '2023-04-28'),
    (4, 2, 'Added 1 new member', 'Organized debate championship', 'Average participation rate: 80%', 'Approved', '2023-08-28'),
    (4, 3, 'No membership changes', 'Planning leadership workshop', 'Expected participation: 85%', 'Draft', '2023-09-18'),
    
    -- Music Club (Club 5) Reports
    (5, 1, 'Added 2 new members', 'Organized annual concert', 'Average participation rate: 88%', 'Approved', '2023-04-29'),
    (5, 2, 'No membership changes', 'Organized open mic night', 'Average participation rate: 75%', 'Approved', '2023-08-29'),
    (5, 3, 'No membership changes', 'Planning music workshop', 'Expected participation: 80%', 'Draft', '2023-09-19'),
    
    -- FPT Chess Club (Club 6) Reports
    (6, 1, 'Added 1 new member', 'Organized chess tournament', 'Average participation rate: 85%', 'Approved', '2023-04-30'),
    (6, 2, 'No membership changes', 'Organized speed chess competition', 'Average participation rate: 80%', 'Approved', '2023-08-30'),
    (6, 3, 'No membership changes', 'Planning chess strategy workshop', 'Expected participation: 75%', 'Draft', '2023-09-20'),
    
    -- FPT Dance Club (Club 7) Reports
    (7, 1, 'Added 3 new members', 'Organized dance showcase', 'Average participation rate: 90%', 'Approved', '2023-05-01'),
    (7, 2, 'Added 1 new member', 'Organized dance workshop', 'Average participation rate: 85%', 'Approved', '2023-08-31'),
    (7, 3, 'No membership changes', 'Planning dance competition', 'Expected participation: 95%', 'Draft', '2023-09-21');
GO
