/* ==========================================================================
   PERSONAL WEBSITE PROJECT
   Script: 02_StoredProcedures.sql
   Purpose: ALL data access goes through these stored procedures.
            No inline SQL is used anywhere in the C# code.
   ========================================================================== */

USE PersonalWebsiteDB;
GO

/* ==========================================================================
   AUTH: sp_User_Login
   Retrieves a user by username/email so C# can verify the password hash.
   (Hash comparison happens in C# — SQL never receives the plain password.)
   ========================================================================== */
IF OBJECT_ID('dbo.sp_User_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_Login;
GO
CREATE PROCEDURE dbo.sp_User_Login
    @UsernameOrEmail NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  U.UserId,
            U.Username,
            U.Email,
            U.PasswordHash,
            U.PasswordSalt,
            U.RoleId,
            R.RoleName,
            U.FullName,
            U.IsActive
    FROM    dbo.Users U
    INNER JOIN dbo.Roles R ON R.RoleId = U.RoleId
    WHERE   U.Username = @UsernameOrEmail OR U.Email = @UsernameOrEmail;
END
GO

/* ==========================================================================
   AUTH: sp_User_UpdateLastLogin
   ========================================================================== */
IF OBJECT_ID('dbo.sp_User_UpdateLastLogin', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_UpdateLastLogin;
GO
CREATE PROCEDURE dbo.sp_User_UpdateLastLogin
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users SET LastLogin = GETDATE() WHERE UserId = @UserId;
END
GO

/* ==========================================================================
   AUTH: sp_User_CheckExists
   Used by Register.aspx to confirm a username/email is not already taken.
   ========================================================================== */
IF OBJECT_ID('dbo.sp_User_CheckExists', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_CheckExists;
GO
CREATE PROCEDURE dbo.sp_User_CheckExists
    @Username NVARCHAR(50),
    @Email    NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS MatchCount
    FROM dbo.Users
    WHERE Username = @Username OR Email = @Email;
END
GO

/* ==========================================================================
   AUTH: sp_User_Signup
   Creates a new user account with role = User (2) by default.
   ========================================================================== */
IF OBJECT_ID('dbo.sp_User_Signup', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_Signup;
GO
CREATE PROCEDURE dbo.sp_User_Signup
    @Username     NVARCHAR(50),
    @Email        NVARCHAR(150),
    @PasswordHash NVARCHAR(256),
    @PasswordSalt NVARCHAR(256),
    @FullName     NVARCHAR(150),
    @NewUserId    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Users (Username, Email, PasswordHash, PasswordSalt, RoleId, FullName, IsActive)
    VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, 2, @FullName, 1);

    SET @NewUserId = SCOPE_IDENTITY();
END
GO

/* ==========================================================================
   PROFILE: sp_User_GetById
   ========================================================================== */
IF OBJECT_ID('dbo.sp_User_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_GetById;
GO
CREATE PROCEDURE dbo.sp_User_GetById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  U.UserId, U.Username, U.Email, U.FullName, U.IsActive,
            U.RoleId, R.RoleName, U.DateCreated, U.LastLogin
    FROM    dbo.Users U
    INNER JOIN dbo.Roles R ON R.RoleId = U.RoleId
    WHERE   U.UserId = @UserId;
END
GO

/* ==========================================================================
   PROFILE: sp_User_UpdateProfile
   Authenticated user modifying their OWN information.
   ========================================================================== */
IF OBJECT_ID('dbo.sp_User_UpdateProfile', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_UpdateProfile;
GO
CREATE PROCEDURE dbo.sp_User_UpdateProfile
    @UserId   INT,
    @FullName NVARCHAR(150),
    @Email    NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users
    SET FullName = @FullName,
        Email    = @Email
    WHERE UserId = @UserId;
END
GO

/* ==========================================================================
   PROFILE: sp_User_ChangePassword
   ========================================================================== */
IF OBJECT_ID('dbo.sp_User_ChangePassword', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_ChangePassword;
GO
CREATE PROCEDURE dbo.sp_User_ChangePassword
    @UserId       INT,
    @PasswordHash NVARCHAR(256),
    @PasswordSalt NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users
    SET PasswordHash = @PasswordHash,
        PasswordSalt = @PasswordSalt
    WHERE UserId = @UserId;
END
GO

/* ==========================================================================
   ADMIN: sp_Admin_GetAllUsers
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Admin_GetAllUsers', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_GetAllUsers;
GO
CREATE PROCEDURE dbo.sp_Admin_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  U.UserId, U.Username, U.Email, U.FullName,
            R.RoleName, U.IsActive, U.DateCreated, U.LastLogin
    FROM    dbo.Users U
    INNER JOIN dbo.Roles R ON R.RoleId = U.RoleId
    ORDER BY U.DateCreated DESC;
END
GO

/* ==========================================================================
   ADMIN: sp_Admin_SetUserActiveStatus
   Tags a user as Active/Inactive (soft toggle instead of hard delete).
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Admin_SetUserActiveStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_SetUserActiveStatus;
GO
CREATE PROCEDURE dbo.sp_Admin_SetUserActiveStatus
    @UserId   INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users SET IsActive = @IsActive WHERE UserId = @UserId;
END
GO

/* ==========================================================================
   ADMIN: sp_Admin_DeleteUser
   Hard delete of a user account (removes related audit logs first).
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Admin_DeleteUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_DeleteUser;
GO
CREATE PROCEDURE dbo.sp_Admin_DeleteUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.AuditLogs WHERE UserId = @UserId;
    DELETE FROM dbo.Users WHERE UserId = @UserId;
END
GO

/* ==========================================================================
   ADMIN: sp_Admin_GetUserStats
   Returns active/inactive counts + total + recent signups for the report page.
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Admin_GetUserStats', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_GetUserStats;
GO
CREATE PROCEDURE dbo.sp_Admin_GetUserStats
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM dbo.Users)                         AS TotalUsers,
        (SELECT COUNT(*) FROM dbo.Users WHERE IsActive = 1)      AS ActiveUsers,
        (SELECT COUNT(*) FROM dbo.Users WHERE IsActive = 0)      AS InactiveUsers,
        (SELECT COUNT(*) FROM dbo.Users WHERE RoleId = 1)        AS AdminCount,
        (SELECT COUNT(*) FROM dbo.Users WHERE RoleId = 2)        AS RegularUserCount,
        (SELECT COUNT(*) FROM dbo.Users
            WHERE DateCreated >= DATEADD(DAY, -7, GETDATE()))    AS SignupsLast7Days;

    -- Daily signup activity for the last 30 days (for a chart)
    SELECT
        CAST(DateCreated AS DATE) AS SignupDate,
        COUNT(*) AS SignupCount
    FROM dbo.Users
    WHERE DateCreated >= DATEADD(DAY, -30, GETDATE())
    GROUP BY CAST(DateCreated AS DATE)
    ORDER BY SignupDate;

    -- Recent activity from AuditLogs
    SELECT TOP 20
        A.ActionType, A.ActionDetail, A.ActionDate, U.Username
    FROM dbo.AuditLogs A
    LEFT JOIN dbo.Users U ON U.UserId = A.UserId
    ORDER BY A.ActionDate DESC;
END
GO

/* ==========================================================================
   CONTENT: sp_Content_GetPersonalDetails
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Content_GetPersonalDetails', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_GetPersonalDetails;
GO
CREATE PROCEDURE dbo.sp_Content_GetPersonalDetails
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
    FROM dbo.PersonalDetails
    ORDER BY PersonalDetailId DESC;
END
GO

/* ==========================================================================
   CONTENT: sp_Content_UpdatePersonalDetails  (Admin content editor)
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Content_UpdatePersonalDetails', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_UpdatePersonalDetails;
GO
CREATE PROCEDURE dbo.sp_Content_UpdatePersonalDetails
    @PersonalDetailId INT,
    @FullName         NVARCHAR(150),
    @Address          NVARCHAR(300),
    @ContactNumber    NVARCHAR(50),
    @Email            NVARCHAR(150),
    @Bio              NVARCHAR(MAX),
    @ModifiedByUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.PersonalDetails
    SET FullName         = @FullName,
        Address          = @Address,
        ContactNumber    = @ContactNumber,
        Email            = @Email,
        Bio              = @Bio,
        LastModifiedBy   = @ModifiedByUserId,
        LastModifiedDate = GETDATE()
    WHERE PersonalDetailId = @PersonalDetailId;
END
GO

/* ==========================================================================
   CONTENT: Education CRUD
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Content_GetEducation', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_GetEducation;
GO
CREATE PROCEDURE dbo.sp_Content_GetEducation
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Education WHERE IsActive = 1 ORDER BY DisplayOrder;
END
GO

IF OBJECT_ID('dbo.sp_Content_UpsertEducation', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_UpsertEducation;
GO
CREATE PROCEDURE dbo.sp_Content_UpsertEducation
    @EducationId  INT = NULL,
    @Level        NVARCHAR(100),
    @SchoolName   NVARCHAR(200),
    @Course       NVARCHAR(200) = NULL,
    @YearFrom     INT = NULL,
    @YearTo       NVARCHAR(20) = NULL,
    @DisplayOrder INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @EducationId IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.Education WHERE EducationId = @EducationId)
    BEGIN
        INSERT INTO dbo.Education (Level, SchoolName, Course, YearFrom, YearTo, DisplayOrder)
        VALUES (@Level, @SchoolName, @Course, @YearFrom, @YearTo, @DisplayOrder);
    END
    ELSE
    BEGIN
        UPDATE dbo.Education
        SET Level = @Level, SchoolName = @SchoolName, Course = @Course,
            YearFrom = @YearFrom, YearTo = @YearTo, DisplayOrder = @DisplayOrder
        WHERE EducationId = @EducationId;
    END
END
GO

IF OBJECT_ID('dbo.sp_Content_DeleteEducation', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_DeleteEducation;
GO
CREATE PROCEDURE dbo.sp_Content_DeleteEducation
    @EducationId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Education SET IsActive = 0 WHERE EducationId = @EducationId;
END
GO

/* ==========================================================================
   CONTENT: Skills CRUD
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Content_GetSkills', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_GetSkills;
GO
CREATE PROCEDURE dbo.sp_Content_GetSkills
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Skills WHERE IsActive = 1 ORDER BY DisplayOrder;
END
GO

IF OBJECT_ID('dbo.sp_Content_UpsertSkill', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_UpsertSkill;
GO
CREATE PROCEDURE dbo.sp_Content_UpsertSkill
    @SkillId      INT = NULL,
    @SkillName    NVARCHAR(150),
    @DisplayOrder INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @SkillId IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.Skills WHERE SkillId = @SkillId)
    BEGIN
        INSERT INTO dbo.Skills (SkillName, DisplayOrder) VALUES (@SkillName, @DisplayOrder);
    END
    ELSE
    BEGIN
        UPDATE dbo.Skills SET SkillName = @SkillName, DisplayOrder = @DisplayOrder
        WHERE SkillId = @SkillId;
    END
END
GO

IF OBJECT_ID('dbo.sp_Content_DeleteSkill', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_DeleteSkill;
GO
CREATE PROCEDURE dbo.sp_Content_DeleteSkill
    @SkillId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Skills SET IsActive = 0 WHERE SkillId = @SkillId;
END
GO

/* ==========================================================================
   CONTENT: Hobbies CRUD
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Content_GetHobbies', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_GetHobbies;
GO
CREATE PROCEDURE dbo.sp_Content_GetHobbies
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Hobbies WHERE IsActive = 1 ORDER BY DisplayOrder;
END
GO

IF OBJECT_ID('dbo.sp_Content_UpsertHobby', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_UpsertHobby;
GO
CREATE PROCEDURE dbo.sp_Content_UpsertHobby
    @HobbyId      INT = NULL,
    @HobbyName    NVARCHAR(150),
    @DisplayOrder INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @HobbyId IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.Hobbies WHERE HobbyId = @HobbyId)
    BEGIN
        INSERT INTO dbo.Hobbies (HobbyName, DisplayOrder) VALUES (@HobbyName, @DisplayOrder);
    END
    ELSE
    BEGIN
        UPDATE dbo.Hobbies SET HobbyName = @HobbyName, DisplayOrder = @DisplayOrder
        WHERE HobbyId = @HobbyId;
    END
END
GO

IF OBJECT_ID('dbo.sp_Content_DeleteHobby', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_DeleteHobby;
GO
CREATE PROCEDURE dbo.sp_Content_DeleteHobby
    @HobbyId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Hobbies SET IsActive = 0 WHERE HobbyId = @HobbyId;
END
GO

/* ==========================================================================
   CONTENT: Projects CRUD ("My Projects" section)
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Content_GetProjects', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_GetProjects;
GO
CREATE PROCEDURE dbo.sp_Content_GetProjects
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Projects WHERE IsActive = 1 ORDER BY DisplayOrder;
END
GO

IF OBJECT_ID('dbo.sp_Content_UpsertProject', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_UpsertProject;
GO
CREATE PROCEDURE dbo.sp_Content_UpsertProject
    @ProjectId    INT = NULL,
    @Title        NVARCHAR(150),
    @Description  NVARCHAR(500),
    @Tech         NVARCHAR(200) = NULL,
    @ProjectUrl   NVARCHAR(300) = NULL,
    @DisplayOrder INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @ProjectId IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.Projects WHERE ProjectId = @ProjectId)
    BEGIN
        INSERT INTO dbo.Projects (Title, Description, Tech, ProjectUrl, DisplayOrder)
        VALUES (@Title, @Description, @Tech, @ProjectUrl, @DisplayOrder);
    END
    ELSE
    BEGIN
        UPDATE dbo.Projects
        SET Title = @Title, Description = @Description, Tech = @Tech,
            ProjectUrl = @ProjectUrl, DisplayOrder = @DisplayOrder
        WHERE ProjectId = @ProjectId;
    END
END
GO

IF OBJECT_ID('dbo.sp_Content_DeleteProject', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Content_DeleteProject;
GO
CREATE PROCEDURE dbo.sp_Content_DeleteProject
    @ProjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Projects SET IsActive = 0 WHERE ProjectId = @ProjectId;
END
GO

/* ==========================================================================
   AUDIT: sp_Audit_InsertLog
   ========================================================================== */
IF OBJECT_ID('dbo.sp_Audit_InsertLog', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Audit_InsertLog;
GO
CREATE PROCEDURE dbo.sp_Audit_InsertLog
    @UserId       INT = NULL,
    @ActionType   NVARCHAR(100),
    @ActionDetail NVARCHAR(500) = NULL,
    @IPAddress    NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AuditLogs (UserId, ActionType, ActionDetail, IPAddress)
    VALUES (@UserId, @ActionType, @ActionDetail, @IPAddress);
END
GO

PRINT 'All stored procedures created successfully.';
GO
