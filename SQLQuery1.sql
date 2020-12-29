PRINT N'Creating Human Resource..';
GO
CREATE SCHEMA [HumanResource]
		AUTHORIZATION [dbo];
GO
PRINT N'Creating HumanResource.Departments..';
GO
CREATE TABLE [HumanResource].[Departments] (
	[DepartmentID] INT IDENTITY(1,1) NOT NULL,
	[DepartmentName] NVARCHAR(40) NOT NULL,
	[ManagerID] INT,
	[LocationID] INT NOT NULL
	);
GO
PRINT N'Creating HumanResource.Locations..';
GO
CREATE TABLE [HumanResource].[Locations] (
	[LocationID] INT IDENTITY(1,1) NOT NULL,
	[LocationName] NVARCHAR(20) NOT NULL,
	[Country] NVARCHAR(30) NOT NULL,
	[City] NVARCHAR(30) NOT NULL,
	[StreetAddress] NVARCHAR(40) NOT NULL
	);
GO
PRINT N'Creating HumanResource.PK_Department_DeptID...';
GO
ALTER TABLE [HumanResource].[Departments]
	ADD CONSTRAINT [PK_Department_DeptID] PRIMARY KEY CLUSTERED ([DepartmentID] ASC)
	WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF,
	STATISTICS_NORECOMPUTE = OFF);
GO
PRINT N'Creating HumanResource.PK_Location_LocID...';
GO
ALTER TABLE [HumanResource].[Locations]
	ADD CONSTRAINT [PK_Locations_LocID] PRIMARY KEY CLUSTERED ([LocationID] ASC)
	WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF,
	STATISTICS_NORECOMPUTE = OFF);

GO  
PRINT N'Creating HumanResource.FK_Departments_Location_LocID...';  
GO  
ALTER TABLE [HumanResource].[Departments]
    ADD CONSTRAINT [FK_Departments_Location_LocID] FOREIGN KEY ([LocationID]) REFERENCES [HumanResource].[Locations] ([LocationID]) ON DELETE NO ACTION ON UPDATE NO ACTION;  

