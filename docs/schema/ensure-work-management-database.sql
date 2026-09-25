IF OBJECT_ID(N'dbo.WorkItems',N'U') IS NULL
BEGIN
CREATE TABLE dbo.Projects(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Projects PRIMARY KEY,
 [Key] nvarchar(80) NOT NULL CONSTRAINT UQ_Projects_Key UNIQUE,
 Name nvarchar(200) NOT NULL, IsEnabled bit NOT NULL);
CREATE TABLE dbo.WorkItems(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItems PRIMARY KEY,
 ProjectId bigint NOT NULL, ParentWorkItemId bigint NULL, [Key] nvarchar(40) NOT NULL,
 Title nvarchar(300) NOT NULL, Domain nvarchar(80) NOT NULL, Status tinyint NOT NULL,
 Priority tinyint NOT NULL, Description nvarchar(max) NULL, OwnerRole nvarchar(120) NULL,
 OwnerAgent nvarchar(200) NULL, ChatId nvarchar(200) NULL, Branch nvarchar(300) NULL,
 CommitSha nvarchar(80) NULL, CreatedAtUtc datetime2(3) NOT NULL, UpdatedAtUtc datetime2(3) NOT NULL,
 StartedAtUtc datetime2(3) NULL, CompletedAtUtc datetime2(3) NULL, AccumulatedSeconds bigint NOT NULL CONSTRAINT DF_WorkItems_AccumulatedSeconds DEFAULT(0),
 CONSTRAINT FK_WorkItems_Projects FOREIGN KEY(ProjectId) REFERENCES dbo.Projects(Id),
 CONSTRAINT FK_WorkItems_Parent FOREIGN KEY(ParentWorkItemId) REFERENCES dbo.WorkItems(Id),
 CONSTRAINT UQ_WorkItems_Project_Key UNIQUE(ProjectId,[Key]));
CREATE TABLE dbo.WorkRoles(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkRoles PRIMARY KEY,
 [Key] nvarchar(120) NOT NULL CONSTRAINT UQ_WorkRoles_Key UNIQUE,
 Name nvarchar(200) NOT NULL, Scope nvarchar(1000) NOT NULL, IsEnabled bit NOT NULL);
CREATE TABLE dbo.WorkItemLogs(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItemLogs PRIMARY KEY,
 WorkItemId bigint NOT NULL, Author nvarchar(200) NOT NULL, Message nvarchar(max) NOT NULL,
 ChatId nvarchar(200) NULL, CreatedAtUtc datetime2(3) NOT NULL,
 CONSTRAINT FK_WorkItemLogs_WorkItems FOREIGN KEY(WorkItemId) REFERENCES dbo.WorkItems(Id));
CREATE TABLE dbo.ChatWorkIntakes(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChatWorkIntakes PRIMARY KEY,
 ChatId nvarchar(200) NOT NULL, Author nvarchar(200) NOT NULL, Message nvarchar(max) NOT NULL,
 WorkItemId bigint NULL, Status tinyint NOT NULL, CreatedAtUtc datetime2(3) NOT NULL,
 CONSTRAINT FK_ChatWorkIntakes_WorkItems FOREIGN KEY(WorkItemId) REFERENCES dbo.WorkItems(Id));
CREATE TABLE dbo.WorkItemDependencies(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItemDependencies PRIMARY KEY,
 WorkItemId bigint NOT NULL, DependsOnWorkItemId bigint NOT NULL, CreatedAtUtc datetime2(3) NOT NULL,
 CONSTRAINT FK_WorkItemDependencies_WorkItem FOREIGN KEY(WorkItemId) REFERENCES dbo.WorkItems(Id),
 CONSTRAINT FK_WorkItemDependencies_DependsOn FOREIGN KEY(DependsOnWorkItemId) REFERENCES dbo.WorkItems(Id));
CREATE TABLE dbo.WorkItemCommits(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItemCommits PRIMARY KEY,
 WorkItemId bigint NOT NULL, Sha nvarchar(80) NOT NULL, Message nvarchar(500) NULL, CreatedAtUtc datetime2(3) NOT NULL,
 CONSTRAINT FK_WorkItemCommits_WorkItems FOREIGN KEY(WorkItemId) REFERENCES dbo.WorkItems(Id));
CREATE TABLE dbo.WorkItemTestEvidence(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItemTestEvidence PRIMARY KEY,
 WorkItemId bigint NOT NULL, TestName nvarchar(300) NOT NULL, Result nvarchar(40) NOT NULL,
 Details nvarchar(max) NULL, CreatedAtUtc datetime2(3) NOT NULL,
 CONSTRAINT FK_WorkItemTestEvidence_WorkItems FOREIGN KEY(WorkItemId) REFERENCES dbo.WorkItems(Id));
CREATE TABLE dbo.WorkItemTimeEntries(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItemTimeEntries PRIMARY KEY,
 WorkItemId bigint NOT NULL, StartedAtUtc datetime2(3) NOT NULL, EndedAtUtc datetime2(3) NULL,
 DurationSeconds bigint NOT NULL CONSTRAINT DF_WorkItemTimeEntries_DurationSeconds DEFAULT(0), Note nvarchar(1000) NULL,
 CONSTRAINT FK_WorkItemTimeEntries_WorkItems FOREIGN KEY(WorkItemId) REFERENCES dbo.WorkItems(Id));
END
GO
IF COL_LENGTH(N'dbo.WorkItems',N'ParentWorkItemId') IS NULL ALTER TABLE dbo.WorkItems ADD ParentWorkItemId bigint NULL;
IF COL_LENGTH(N'dbo.WorkItems',N'StartedAtUtc') IS NULL ALTER TABLE dbo.WorkItems ADD StartedAtUtc datetime2(3) NULL;
IF COL_LENGTH(N'dbo.WorkItems',N'CompletedAtUtc') IS NULL ALTER TABLE dbo.WorkItems ADD CompletedAtUtc datetime2(3) NULL;
IF COL_LENGTH(N'dbo.WorkItems',N'AccumulatedSeconds') IS NULL ALTER TABLE dbo.WorkItems ADD AccumulatedSeconds bigint NOT NULL CONSTRAINT DF_WorkItems_AccumulatedSeconds DEFAULT(0);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_WorkItems_Parent') ALTER TABLE dbo.WorkItems ADD CONSTRAINT FK_WorkItems_Parent FOREIGN KEY(ParentWorkItemId) REFERENCES dbo.WorkItems(Id);
IF OBJECT_ID(N'dbo.WorkItemTimeEntries',N'U') IS NULL
BEGIN
 CREATE TABLE dbo.WorkItemTimeEntries(
  Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItemTimeEntries PRIMARY KEY,
  WorkItemId bigint NOT NULL, StartedAtUtc datetime2(3) NOT NULL, EndedAtUtc datetime2(3) NULL,
  DurationSeconds bigint NOT NULL CONSTRAINT DF_WorkItemTimeEntries_DurationSeconds DEFAULT(0), Note nvarchar(1000) NULL,
  CONSTRAINT FK_WorkItemTimeEntries_WorkItems FOREIGN KEY(WorkItemId) REFERENCES dbo.WorkItems(Id));
END
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Projects WHERE [Key]=N'HYPER') INSERT dbo.Projects([Key],Name,IsEnabled) VALUES(N'HYPER',N'Hyper',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'architecture-lead') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'architecture-lead',N'Architecture Lead',N'Architecture, contracts, cross-domain decisions',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'basalam-integration') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'basalam-integration',N'Basalam Integration',N'OAuth, webhooks, provider adapter',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'accounting-platform') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'accounting-platform',N'Accounting Platform',N'Hyperyek accounting API and handlers',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'worker-operations') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'worker-operations',N'Worker Operations',N'Queue, outbox, retry, worker',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'quality') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'quality',N'Quality and Test',N'Integration, E2E and regression tests',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'panel-operations') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'panel-operations',N'Panel Operations',N'Admin dashboard, kanban and CRUD',1);
