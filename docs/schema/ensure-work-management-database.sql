IF OBJECT_ID(N'dbo.WorkItems',N'U') IS NULL
BEGIN
CREATE TABLE dbo.WorkItems(
 Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkItems PRIMARY KEY,
 [Key] nvarchar(40) NOT NULL CONSTRAINT UQ_WorkItems_Key UNIQUE,
 Title nvarchar(300) NOT NULL, Domain nvarchar(80) NOT NULL, Status tinyint NOT NULL,
 Priority tinyint NOT NULL, Description nvarchar(max) NULL, OwnerRole nvarchar(120) NULL,
 OwnerAgent nvarchar(200) NULL, ChatId nvarchar(200) NULL, Branch nvarchar(300) NULL,
 CommitSha nvarchar(80) NULL, CreatedAtUtc datetime2(3) NOT NULL, UpdatedAtUtc datetime2(3) NOT NULL);
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
END
GO
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'architecture-lead') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'architecture-lead',N'Architecture Lead',N'Architecture, contracts, cross-domain decisions',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'basalam-integration') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'basalam-integration',N'Basalam Integration',N'OAuth, webhooks, provider adapter',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'accounting-platform') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'accounting-platform',N'Accounting Platform',N'Hyperyek accounting API and handlers',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'worker-operations') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'worker-operations',N'Worker Operations',N'Queue, outbox, retry, worker',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'quality') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'quality',N'Quality and Test',N'Integration, E2E and regression tests',1);
IF NOT EXISTS (SELECT 1 FROM dbo.WorkRoles WHERE [Key]=N'panel-operations') INSERT dbo.WorkRoles([Key],Name,Scope,IsEnabled) VALUES(N'panel-operations',N'Panel Operations',N'Admin dashboard, kanban and CRUD',1);
