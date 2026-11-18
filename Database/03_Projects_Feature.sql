/* ==========================================================================
   PERSONAL WEBSITE PROJECT
   Script: 03_Projects_Feature.sql
   Purpose: Adds the "My Projects" portfolio section (ported over from the
            static portfolio site) as a new, admin-editable content type.

   IMPORTANT: This is an ADDITIVE script only. It does NOT drop or touch
   the Users, Roles, PersonalDetails, Education, Skills, Hobbies, or
   AuditLogs tables — your existing accounts and data are untouched.
   Just run this once against your existing PersonalWebsiteDB database.
   ========================================================================== */

USE PersonalWebsiteDB;
GO

/* ==========================================================================
   TABLE: Projects (Dynamic website content - "My Projects")
   ========================================================================== */
IF OBJECT_ID('dbo.Projects', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Projects
    (
        ProjectId       INT IDENTITY(1,1) PRIMARY KEY,
        Title           NVARCHAR(150) NOT NULL,
        Description     NVARCHAR(500) NOT NULL,
        TechTags        NVARCHAR(300) NULL,      -- e.g. "ASP.NET, C#, SQL Server"
        DisplayOrder    INT NOT NULL DEFAULT 0,
        IsActive        BIT NOT NULL DEFAULT 1
    );

    -- Seed with the same sample projects from the static portfolio, so the
    -- migration continues showing the same content until you edit it.
    INSERT INTO dbo.Projects (Title, Description, TechTags, DisplayOrder) VALUES
    ('Task Manager App', 'A simple task tracker with due dates and priority levels.', 'ASP.NET, C#, SQL Server', 1),
    ('Recipe Finder', 'Search and save recipes based on ingredients you already have.', 'C#, JavaScript, REST API', 2),
    ('Budget Tracker', 'Personal finance dashboard with charts for spending trends.', 'ASP.NET MVC, Chart.js', 3),
    ('Company Intranet', 'Internal portal for document sharing and team announcements.', 'ASP.NET Web Forms, SQL Server', 4);

    PRINT 'Projects table created and seeded.';
END
ELSE
BEGIN
    PRINT 'Projects table already exists — skipping creation.';
END
GO

/* ==========================================================================
   STORED PROCEDURES: Projects CRUD
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
    @TechTags     NVARCHAR(300) = NULL,
    @DisplayOrder INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @ProjectId IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.Projects WHERE ProjectId = @ProjectId)
    BEGIN
        INSERT INTO dbo.Projects (Title, Description, TechTags, DisplayOrder)
        VALUES (@Title, @Description, @TechTags, @DisplayOrder);
    END
    ELSE
    BEGIN
        UPDATE dbo.Projects
        SET Title = @Title, Description = @Description, TechTags = @TechTags, DisplayOrder = @DisplayOrder
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

PRINT 'Projects feature stored procedures created successfully.';
GO
