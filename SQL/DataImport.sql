USE ClubManagement;
GO

-- Insert data into Clubs table
INSERT INTO Clubs (ClubName, Description, EstablishedDate)
VALUES 
    ('F-Code', 'Programming club for FPT students', '2020-01-15'),
    ('F-Multimedia', 'Multimedia and design club', '2019-05-20'),
    ('FPT Basketball', 'Basketball club for sports enthusiasts', '2018-09-10'),
    ('English Speaking Club', 'Club for improving English speaking skills', '2021-02-28'),
    ('Music Club', 'For music lovers and performers', '2019-11-12');
GO

-- Insert data into Users table
-- First create an admin user
INSERT INTO Users (StudentID, FullName, Email, Password, Role, ClubID, JoinDate, Status)
VALUES 
    (NULL, 'System Admin', 'admin@fpt.edu.vn', 'admin123', 'Admin', NULL, '2020-01-01', 1);

-- Insert club presidents and members
INSERT INTO Users (StudentID, FullName, Email, Password, Role, ClubID, JoinDate, Status)
VALUES
    -- F-Code members
    ('SE12345', 'Nguyen Van A', 'anv@fpt.edu.vn', 'password123', 'President', 1, '2020-02-15', 1),
    ('SE12346', 'Tran Thi B', 'btt@fpt.edu.vn', 'password123', 'VicePresident', 1, '2020-03-10', 1),
    ('SE12347', 'Le Van C', 'clv@fpt.edu.vn', 'password123', 'TeamLeader', 1, '2020-04-05', 1),
    ('SE12348', 'Pham Thi D', 'dpt@fpt.edu.vn', 'password123', 'Member', 1, '2020-05-20', 1),
    ('SE12349', 'Hoang Van E', 'ehv@fpt.edu.vn', 'password123', 'Member', 1, '2020-06-15', 1),
    
    -- F-Multimedia members
    ('GD12345', 'Nguyen Thi F', 'fnt@fpt.edu.vn', 'password123', 'President', 2, '2020-02-20', 1),
    ('GD12346', 'Tran Van G', 'gtv@fpt.edu.vn', 'password123', 'VicePresident', 2, '2020-03-15', 1),
    ('GD12347', 'Le Thi H', 'hlt@fpt.edu.vn', 'password123', 'Member', 2, '2020-04-10', 1),
    
    -- FPT Basketball members
    ('SE23456', 'Pham Van I', 'ipv@fpt.edu.vn', 'password123', 'President', 3, '2020-02-25', 1),
    ('SE23457', 'Hoang Thi J', 'jht@fpt.edu.vn', 'password123', 'Member', 3, '2020-03-20', 1),
    ('SE23458', 'Nguyen Van K', 'knv@fpt.edu.vn', 'password123', 'Member', 3, '2020-04-15', 1),
    
    -- English Speaking Club members
    ('BA12345', 'Tran Thi L', 'ltt@fpt.edu.vn', 'password123', 'President', 4, '2021-03-01', 1),
    ('BA12346', 'Le Van M', 'mlv@fpt.edu.vn', 'password123', 'VicePresident', 4, '2021-03-15', 1),
    ('BA12347', 'Pham Thi N', 'npt@fpt.edu.vn', 'password123', 'Member', 4, '2021-04-01', 1),
    
    -- Music Club members
    ('MC12345', 'Hoang Van O', 'ohv@fpt.edu.vn', 'password123', 'President', 5, '2019-12-01', 1),
    ('MC12346', 'Nguyen Thi P', 'pnt@fpt.edu.vn', 'password123', 'Member', 5, '2020-01-15', 1),
    ('MC12347', 'Tran Van Q', 'qtv@fpt.edu.vn', 'password123', 'Member', 5, '2020-02-01', 0); -- Inactive member
GO

-- Insert data into Semesters table
INSERT INTO Semesters (SemesterName, StartDate, EndDate)
VALUES 
    ('Spring 2023', '2023-01-01', '2023-04-30'),
    ('Summer 2023', '2023-05-01', '2023-08-31'),
    ('Fall 2023', '2023-09-01', '2023-12-31'),
    ('Spring 2024', '2024-01-01', '2024-04-30'),
    ('Summer 2024', '2024-05-01', '2024-08-31'),
    ('Fall 2024', '2024-09-01', '2024-12-31'),
    ('Spring 2025', '2025-01-01', '2025-04-30');
GO

-- Insert data into Events table
INSERT INTO Events (EventName, Description, EventDate, Location, ClubID, Capacity, Status)
VALUES 
    ('Coding Competition', 'Annual coding competition for all students', '2023-03-15 09:00:00', 'Alpha Building', 1, 100, 'Completed'),
    ('Design Workshop', 'Workshop on UI/UX design principles', '2023-06-20 14:00:00', 'Beta Building', 2, 50, 'Completed'),
    ('Basketball Tournament', 'Inter-university basketball tournament', '2023-10-10 08:00:00', 'FPT Stadium', 3, 200, 'Completed'),
    ('English Speaking Contest', 'Contest to improve English speaking skills', '2024-02-05 13:00:00', 'Gamma Building', 4, 80, 'Completed'),
    ('Music Festival', 'Annual music festival featuring student bands', '2024-04-25 18:00:00', 'FPT Auditorium', 5, 300, 'Completed'),
    
    ('Hackathon 2024', 'A 48-hour coding challenge', '2024-06-15 08:00:00', 'Alpha Building', 1, 120, 'Completed'),
    ('Digital Art Exhibition', 'Exhibition of student digital artwork', '2024-07-10 10:00:00', 'Art Gallery', 2, 150, 'Completed'),
    ('Friendly Basketball Match', 'Friendly match with local university', '2024-08-20 15:00:00', 'FPT Stadium', 3, 100, 'Completed'),
    ('Public Speaking Workshop', 'Workshop to improve public speaking skills', '2024-09-05 14:00:00', 'Delta Building', 4, 60, 'Completed'),
    ('Karaoke Night', 'Fun night of karaoke for all students', '2024-10-15 19:00:00', 'Student Center', 5, 80, 'Completed'),
    
    ('Spring Coding Bootcamp', 'Intensive coding bootcamp for beginners', '2025-02-10 09:00:00', 'Alpha Building', 1, 50, 'Completed'),
    ('UI/UX Masterclass', 'Advanced UI/UX design techniques', '2025-03-05 13:00:00', 'Beta Building', 2, 40, 'Completed'),
    ('Basketball Training Camp', 'Training camp for new players', '2025-03-15 10:00:00', 'FPT Stadium', 3, 30, 'Ongoing'),
    ('English Debate Competition', 'Debate competition on current topics', '2025-04-10 14:00:00', 'Gamma Building', 4, 60, 'Upcoming'),
    ('Spring Concert', 'Spring concert featuring student performances', '2025-04-30 18:00:00', 'FPT Auditorium', 5, 250, 'Upcoming');
GO

-- Insert data into EventParticipants table
INSERT INTO EventParticipants (EventID, UserID, Status, RegistrationDate)
VALUES 
    -- Coding Competition participants
    (1, 2, 'Attended', '2023-03-01 10:00:00'),
    (1, 3, 'Attended', '2023-03-02 11:30:00'),
    (1, 4, 'Attended', '2023-03-03 09:45:00'),
    (1, 5, 'Absent', '2023-03-01 14:20:00'),
    
    -- Design Workshop participants
    (2, 7, 'Attended', '2023-06-10 08:30:00'),
    (2, 8, 'Attended', '2023-06-11 10:15:00'),
    (2, 4, 'Attended', '2023-06-12 13:40:00'), -- F-Code member attending F-Multimedia event
    
    -- Basketball Tournament participants
    (3, 10, 'Attended', '2023-10-01 09:00:00'),
    (3, 11, 'Attended', '2023-10-02 11:20:00'),
    (3, 12, 'Absent', '2023-10-03 14:45:00'),
    
    -- English Speaking Contest participants
    (4, 13, 'Attended', '2024-01-20 10:30:00'),
    (4, 14, 'Attended', '2024-01-21 13:15:00'),
    (4, 15, 'Attended', '2024-01-22 15:40:00'),
    (4, 3, 'Attended', '2024-01-23 09:50:00'), -- F-Code member attending English club event
    
    -- Music Festival participants
    (5, 16, 'Attended', '2024-04-10 11:00:00'),
    (5, 17, 'Attended', '2024-04-11 14:25:00'),
    (5, 18, 'Absent', '2024-04-12 16:30:00'),
    
    -- Hackathon 2024 participants
    (6, 2, 'Attended', '2024-06-01 09:15:00'),
    (6, 3, 'Attended', '2024-06-02 10:45:00'),
    (6, 4, 'Attended', '2024-06-03 13:20:00'),
    (6, 5, 'Attended', '2024-06-04 15:10:00'),
    
    -- Current and upcoming events
    (13, 10, 'Registered', '2025-03-01 10:00:00'),
    (13, 11, 'Registered', '2025-03-02 11:30:00'),
    (14, 13, 'Registered', '2025-03-20 09:45:00'),
    (14, 14, 'Registered', '2025-03-21 14:20:00'),
    (15, 16, 'Registered', '2025-04-01 08:30:00'),
    (15, 17, 'Registered', '2025-04-02 10:15:00');
GO

-- Insert data into Reports table
INSERT INTO Reports (ClubID, SemesterID, MemberChanges, EventSummary, ParticipationStats, ReportStatus, CreatedDate)
VALUES 
    (1, 1, 'Added 3 new members', 'Successfully organized Coding Competition with 80% attendance', 'Overall participation rate: 85%', 'Approved', '2023-04-25'),
    (2, 1, 'Added 1 new member', 'Design Workshop received positive feedback', 'Overall participation rate: 90%', 'Approved', '2023-04-26'),
    (3, 1, 'No changes in membership', 'Planned for upcoming Basketball Tournament', 'N/A', 'Approved', '2023-04-27'),
    
    (1, 2, 'No changes in membership', 'No events organized this semester', 'N/A', 'Approved', '2023-08-25'),
    (2, 2, 'Lost 1 member due to graduation', 'No events organized this semester', 'N/A', 'Approved', '2023-08-26'),
    
    (3, 3, 'Added 2 new members', 'Basketball Tournament was a great success', 'Overall participation rate: 95%', 'Approved', '2023-12-20'),
    (4, 3, 'Added 1 new member', 'Prepared for English Speaking Contest', 'N/A', 'Approved', '2023-12-21'),
    
    (4, 4, 'No changes in membership', 'English Speaking Contest had 100% attendance', 'Overall participation rate: 100%', 'Approved', '2024-04-25'),
    (5, 4, 'Lost 1 member due to inactivity', 'Music Festival attracted 250 attendees', 'Overall participation rate: 83%', 'Approved', '2024-04-26'),
    
    (1, 5, 'Added 2 new members', 'Hackathon 2024 was a great success with 100 participants', 'Overall participation rate: 100%', 'Approved', '2024-08-25'),
    (2, 5, 'No changes in membership', 'Digital Art Exhibition showcased 50 artworks', 'Overall participation rate: 95%', 'Approved', '2024-08-26'),
    (3, 5, 'Added 1 new member', 'Friendly Basketball Match ended in a win', 'Overall participation rate: 90%', 'Approved', '2024-08-27'),
    
    (4, 6, 'No changes in membership', 'Public Speaking Workshop had positive feedback', 'Overall participation rate: 92%', 'Submitted', '2024-12-15'),
    (5, 6, 'No changes in membership', 'Karaoke Night was fun and engaging', 'Overall participation rate: 88%', 'Submitted', '2024-12-16'),
    
    (1, 7, 'Added 1 new member', 'Spring Coding Bootcamp trained 45 students', 'Overall participation rate: 90%', 'Draft', '2025-03-10'),
    (2, 7, 'No changes in membership', 'UI/UX Masterclass was well-received', 'Overall participation rate: 95%', 'Draft', '2025-03-12');
GO

-- Insert data into Teams table
INSERT INTO Teams (TeamName, Description, ClubID, TeamLeaderID)
VALUES 
    ('Web Development', 'Team focused on web development projects', 1, 4),
    ('Mobile Development', 'Team focused on mobile app development', 1, 5),
    ('Graphic Design', 'Team focused on graphic design projects', 2, 8),
    ('Video Production', 'Team focused on video production', 2, 7),
    ('Men''s Team', 'Men''s basketball team', 3, 10),
    ('Women''s Team', 'Women''s basketball team', 3, 11),
    ('Debate Team', 'Team focused on debate competitions', 4, 14),
    ('Pronunciation Team', 'Team focused on pronunciation practice', 4, 15),
    ('Band', 'Live performance band', 5, 16),
    ('Choir', 'Vocal ensemble', 5, 17);
GO

-- Insert data into TeamMembers table
INSERT INTO TeamMembers (TeamID, UserID, JoinDate)
VALUES 
    -- Web Development team members
    (1, 2, '2020-03-01'), -- President is also in the team
    (1, 3, '2020-03-15'),
    (1, 4, '2020-04-10'), -- Team leader
    
    -- Mobile Development team members
    (2, 2, '2020-03-02'), -- President is also in this team
    (2, 5, '2020-06-20'), -- Team leader
    
    -- Graphic Design team members
    (3, 6, '2020-03-01'),
    (3, 7, '2020-03-20'),
    (3, 8, '2020-04-15'), -- Team leader
    
    -- Video Production team members
    (4, 6, '2020-03-02'),
    (4, 7, '2020-03-25'), -- Team leader
    
    -- Men's Team members
    (5, 9, '2020-03-01'),
    (5, 10, '2020-03-25'), -- Team leader
    (5, 12, '2020-04-20'),
    
    -- Women's Team members
    (6, 9, '2020-03-02'),
    (6, 11, '2020-03-30'), -- Team leader
    
    -- Debate Team members
    (7, 13, '2021-03-10'),
    (7, 14, '2021-03-25'), -- Team leader
    
    -- Pronunciation Team members
    (8, 13, '2021-03-15'),
    (8, 15, '2021-04-05'), -- Team leader
    
    -- Band members
    (9, 16, '2020-01-10'), -- Team leader
    (9, 17, '2020-01-20'),
    
    -- Choir members
    (10, 16, '2020-01-15'),
    (10, 17, '2020-01-25'),
    (10, 18, '2020-02-10');
GO
