/* ==========================================================================
   PERSONAL WEBSITE PROJECT
   ASP.NET Web Forms (C#) + MS SQL Server
   Script: 01_Schema_And_SeedData.sql
   Purpose: Creates the database, all tables, and seeds initial data.
   ========================================================================== */

IF DB_ID('PersonalWebsiteDB') IS NULL
BEGIN
    CREATE DATABASE PersonalWebsiteDB;
END
GO

USE PersonalWebsiteDB;
GO

/* ==========================================================================
   1. ROLES TABLE
   ========================================================================== */
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO
CREATE TABLE dbo.Roles
(
    RoleId      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(50) NOT NULL UNIQUE
);
GO

/* ==========================================================================
   2. USERS TABLE
   Passwords are stored as salted hashes (PBKDF2), never plain text.
   ========================================================================== */
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO
CREATE TABLE dbo.Users
(
    UserId          INT IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(50)  NOT NULL UNIQUE,
    Email           NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(256) NOT NULL,
    PasswordSalt    NVARCHAR(256) NOT NULL,
    RoleId          INT NOT NULL DEFAULT 2,           -- 1 = Admin, 2 = User
    FullName        NVARCHAR(150) NULL,
    IsActive        BIT NOT NULL DEFAULT 1,            -- Admin can tag Inactive
    DateCreated     DATETIME NOT NULL DEFAULT GETDATE(),
    LastLogin       DATETIME NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
);
GO

/* ==========================================================================
   3. PERSONAL DETAILS TABLE (Dynamic website content - Personal Information)
   Owned by a user (the site owner) but editable by Admin via Content Editor.
   ========================================================================== */
IF OBJECT_ID('dbo.PersonalDetails', 'U') IS NOT NULL DROP TABLE dbo.PersonalDetails;
GO
CREATE TABLE dbo.PersonalDetails
(
    PersonalDetailId INT IDENTITY(1,1) PRIMARY KEY,
    FullName         NVARCHAR(150) NOT NULL,
    Address          NVARCHAR(300) NULL,
    ContactNumber    NVARCHAR(50)  NULL,
    Email            NVARCHAR(150) NULL,
    Bio              NVARCHAR(MAX) NULL,
    ProfileImagePath NVARCHAR(300) NULL,
    LastModifiedBy   INT NULL,
    LastModifiedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PersonalDetails_Users FOREIGN KEY (LastModifiedBy) REFERENCES dbo.Users(UserId)
);
GO

/* ==========================================================================
   4. EDUCATION TABLE (Dynamic website content - Educational Attainment)
   ========================================================================== */
IF OBJECT_ID('dbo.Education', 'U') IS NOT NULL DROP TABLE dbo.Education;
GO
CREATE TABLE dbo.Education
(
    EducationId     INT IDENTITY(1,1) PRIMARY KEY,
    Level           NVARCHAR(100) NOT NULL,   -- e.g. Elementary, Junior High, Senior High, College
    SchoolName      NVARCHAR(200) NOT NULL,
    Course          NVARCHAR(200) NULL,       -- e.g. BS Information Technology
    YearFrom        INT NULL,
    YearTo          NVARCHAR(20) NULL,        -- allows "Present"
    DisplayOrder    INT NOT NULL DEFAULT 0,
    IsActive        BIT NOT NULL DEFAULT 1
);
GO

/* ==========================================================================
   5. SKILLS TABLE (Dynamic website content - Skills)
   ========================================================================== */
IF OBJECT_ID('dbo.Skills', 'U') IS NOT NULL DROP TABLE dbo.Skills;
GO
CREATE TABLE dbo.Skills
(
    SkillId         INT IDENTITY(1,1) PRIMARY KEY,
    SkillName       NVARCHAR(150) NOT NULL,
    DisplayOrder    INT NOT NULL DEFAULT 0,
    IsActive        BIT NOT NULL DEFAULT 1
);
GO

/* ==========================================================================
   6. HOBBIES TABLE (Dynamic website content - Hobbies and Interest)
   ========================================================================== */
IF OBJECT_ID('dbo.Hobbies', 'U') IS NOT NULL DROP TABLE dbo.Hobbies;
GO
CREATE TABLE dbo.Hobbies
(
    HobbyId         INT IDENTITY(1,1) PRIMARY KEY,
    HobbyName       NVARCHAR(150) NOT NULL,
    DisplayOrder    INT NOT NULL DEFAULT 0,
    IsActive        BIT NOT NULL DEFAULT 1
);
GO

/* ==========================================================================
   7. PROJECTS TABLE (Dynamic website content - My Projects)
   ========================================================================== */
IF OBJECT_ID('dbo.Projects', 'U') IS NOT NULL DROP TABLE dbo.Projects;
GO
CREATE TABLE dbo.Projects
(
    ProjectId       INT IDENTITY(1,1) PRIMARY KEY,
    Title           NVARCHAR(150) NOT NULL,
    Description     NVARCHAR(500) NOT NULL,
    Tech            NVARCHAR(200) NULL,        -- e.g. "ASP.NET, C#, SQL Server" (shown as a tag)
    ProjectUrl      NVARCHAR(300) NULL,         -- optional link to a live demo / repo
    DisplayOrder    INT NOT NULL DEFAULT 0,
    IsActive        BIT NOT NULL DEFAULT 1
);
GO

/* ==========================================================================
   8. AUDIT LOGS TABLE
   ========================================================================== */
IF OBJECT_ID('dbo.AuditLogs', 'U') IS NOT NULL DROP TABLE dbo.AuditLogs;
GO
CREATE TABLE dbo.AuditLogs
(
    AuditLogId      INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NULL,
    ActionType      NVARCHAR(100) NOT NULL,   -- e.g. LOGIN, SIGNUP, UPDATE_PROFILE, DEACTIVATE_USER
    ActionDetail    NVARCHAR(500) NULL,
    ActionDate      DATETIME NOT NULL DEFAULT GETDATE(),
    IPAddress       NVARCHAR(50) NULL,
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
);
GO

/* ==========================================================================
   SEED: ROLES
   ========================================================================== */
INSERT INTO dbo.Roles (RoleName) VALUES ('Admin'), ('User');
GO

/* ==========================================================================
   NO USER ACCOUNTS ARE SEEDED HERE ON PURPOSE.

   Passwords are hashed with PBKDF2 in C# (see App_Code/PasswordHelper.cs), so a
   real, working hash can only be produced by running the application - a hash
   typed directly into SQL would not match anything the login page can verify.

   To get your first working accounts, after deploying the app:
     1. Open Register.aspx in the browser and sign up normally
        (this becomes your regular "User" account).
     2. To make that same account (or a second one) an Admin, run:

           UPDATE dbo.Users SET RoleId = 1 WHERE Username = 'your_username';

     3. Log out and back in - you'll now see the "Admin Panel" link in the navbar.

   This is also explained in the setup instructions provided with the project.
   ========================================================================== */

/* ==========================================================================
   SEED: PERSONAL DETAILS
   ========================================================================== */
INSERT INTO dbo.PersonalDetails (FullName, Address, ContactNumber, Email, Bio, LastModifiedBy)
VALUES (
    'KARL IVAN B. LUARCA',
    '#3 Scout Alcaraz St. Brgy. N.S Amoranto, La Loma, Quezon City, Metro Manila, Philippines',
    '09196599632',
    'luarca.karl.ivan.biason@gmail.com',
    'I am a third-year Bachelor of Science in Information Technology student with an interest in computer hardware, troubleshooting, and software development. I enjoy taking computers apart, understanding how their components work, troubleshooting technical problems, and experimenting with different technologies. At the same time, I am developing my programming skills and exploring areas such as web development and database management. I am still learning, but I enjoy challenging myself with new projects and continuously improving my technical skills.',
    NULL
);
GO

/* ==========================================================================
   SEED: EDUCATION
   ========================================================================== */
INSERT INTO dbo.Education (Level, SchoolName, Course, YearFrom, YearTo, DisplayOrder) VALUES
('College',        'Current University', 'Bachelor of Science in Information Technology', 2024, 'Present', 1),
('College',        'Urdaneta City University', 'Bachelor of Science in Information Technology', 2022, '2023', 2),
('Senior High School', 'Arellano University Juan Sumulong Campus', NULL, 2020, '2022', 3),
('Junior High School', 'Mary Liane''s School', NULL, 2016, '2020', 4),
('Elementary',      'Mary Liane''s School', NULL, 2010, '2016', 5);
GO

/* ==========================================================================
   SEED: SKILLS
   ========================================================================== */
INSERT INTO dbo.Skills (SkillName, DisplayOrder) VALUES
('Basic computer literacy skills', 1),
('Organizational skills', 2),
('Strategic planning and scheduling skills', 3),
('Time-management skills', 4),
('Verbal and written communication skills', 5);
GO

/* ==========================================================================
   SEED: HOBBIES
   ========================================================================== */
INSERT INTO dbo.Hobbies (HobbyName, DisplayOrder) VALUES
('Software Development', 1),
('Technology Exploration', 2),
('Problem Solving', 3),
('Strategic Planning', 4);
GO

/* ==========================================================================
   SEED: PROJECTS (carried over from the earlier static portfolio)
   ========================================================================== */
INSERT INTO dbo.Projects (Title, Description, Tech, DisplayOrder) VALUES
('Task Manager App', 'A simple task tracker with due dates and priority levels.', 'ASP.NET, C#, SQL Server', 1),
('Recipe Finder', 'Search and save recipes based on ingredients you already have.', 'C#, JavaScript, REST API', 2),
('Budget Tracker', 'Personal finance dashboard with charts for spending trends.', 'ASP.NET MVC, Chart.js', 3),
('Company Intranet', 'Internal portal for document sharing and team announcements.', 'ASP.NET Web Forms, SQL Server', 4);
GO

PRINT 'Schema created and seed data inserted successfully.';
GO
