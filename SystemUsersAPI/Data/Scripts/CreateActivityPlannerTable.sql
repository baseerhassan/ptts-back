CREATE TABLE [dbo].[ActivityPlanner] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Date] DATE NOT NULL,
    [Time] NVARCHAR(50) NULL,
    [CourseId] INT NOT NULL,
    [ActivityId] INT NOT NULL,
    [Remarks] NVARCHAR(MAX) NULL,
    [Instructor] NVARCHAR(100) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [CreatedDate] DATETIME NOT NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_ActivityPlanner] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivityPlanner_Course] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([CourseId]),
    CONSTRAINT [FK_ActivityPlanner_BasicActivity] FOREIGN KEY ([ActivityId]) REFERENCES [dbo].[BasicActivity] ([Id])
);

CREATE INDEX [IX_ActivityPlanner_CourseId] ON [dbo].[ActivityPlanner] ([CourseId]);
CREATE INDEX [IX_ActivityPlanner_ActivityId] ON [dbo].[ActivityPlanner] ([ActivityId]);