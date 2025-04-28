CREATE TABLE [dbo].[Attendance] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ActivityId] INT NOT NULL,
    [CourseId] INT NOT NULL,
    [TraineeId] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_Attendance] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Attendance_BasicActivity] FOREIGN KEY ([ActivityId]) REFERENCES [dbo].[BasicActivity] ([Id]),
    CONSTRAINT [FK_Attendance_Course] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([CourseId]),
    CONSTRAINT [FK_Attendance_Trainee] FOREIGN KEY ([TraineeId]) REFERENCES [dbo].[Trainee] ([Id]),
    CONSTRAINT [UQ_Attendance_ActivityCourseTrainee] UNIQUE NONCLUSTERED ([ActivityId], [CourseId], [TraineeId])
);

CREATE NONCLUSTERED INDEX [IX_Attendance_ActivityId] ON [dbo].[Attendance]([ActivityId]);
CREATE NONCLUSTERED INDEX [IX_Attendance_CourseId] ON [dbo].[Attendance]([CourseId]);
CREATE NONCLUSTERED INDEX [IX_Attendance_TraineeId] ON [dbo].[Attendance]([TraineeId]);