IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BasicCourse]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BasicCourse] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [CourseName] NVARCHAR(100) NOT NULL,
        [IsActive] BIT NOT NULL,
        [CreatedBy] NVARCHAR(50) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        [ModifiedBy] NVARCHAR(50) NULL,
        [ModifiedDate] DATETIME NULL
    )

    CREATE UNIQUE NONCLUSTERED INDEX [IX_BasicCourse_CourseName] ON [dbo].[BasicCourse]([CourseName]) 
    WHERE [IsActive] = 1
END

