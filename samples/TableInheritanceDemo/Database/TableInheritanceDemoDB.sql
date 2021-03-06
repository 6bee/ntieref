USE [master]
GO

PRINT '## DROP DATABASE #######################################################'
GO
IF DB_ID (N'TableInheritanceDemoDB') IS NOT NULL
BEGIN
    PRINT '   DROP DATABASE [TableInheritanceDemoDB]'
    EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'TableInheritanceDemoDB'
    ALTER DATABASE [TableInheritanceDemoDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [TableInheritanceDemoDB];
END

PRINT '## CREATE DATABASE #######################################################'
GO
DECLARE @sql NVARCHAR(1024), @path VARCHAR(256)

SELECT @path = PHYSICAL_NAME FROM sys.master_files WHERE database_id = DB_ID(N'master') AND TYPE_DESC = 'ROWS'
SET @path = REVERSE(RIGHT(REVERSE(@path),(LEN(@path)-CHARINDEX('\\', REVERSE(@path),1))+1))

PRINT '   CREATE DATABASE [TableInheritanceDemoDB]'
PRINT '   '+@path+'TableInheritanceDemoDB.mdf'
PRINT '   '+@path+'TableInheritanceDemoDB_log.ldf'

SET @sql = 
N'CREATE DATABASE [TableInheritanceDemoDB] 
  CONTAINMENT = NONE 
  ON  PRIMARY 
  ( NAME = N''TableInheritanceDemoDB'', FILENAME = N'''+@path+N'TableInheritanceDemoDB.mdf'' , SIZE = 3136KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) 
  LOG ON 
  ( NAME = N''TableInheritanceDemoDB_Log'', FILENAME = N'''+@path+N'TableInheritanceDemoDB_log.ldf'' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
EXEC sp_executesql @sql

ALTER DATABASE [TableInheritanceDemoDB] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TableInheritanceDemoDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TableInheritanceDemoDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET  MULTI_USER 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TableInheritanceDemoDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TableInheritanceDemoDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [TableInheritanceDemoDB]
GO


--PRINT '## CREATE SCHEMA #######################################################'
--GO
--PRINT '   CREATE SCHEMA [dbo]'
--GO
--CREATE SCHEMA [dbo]
--GO


PRINT '## CREATE TABLES #######################################################'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

PRINT '   CREATE TABLE [dbo].[Addresses]'
GO
CREATE TABLE [dbo].[Addresses](
    [Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Street] [nvarchar](50) NULL,
    [Zip] [nvarchar](20) NULL,
    [City] [nvarchar](250) NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[People]'
GO
CREATE TABLE [dbo].[People](
    [Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [FirstName] [nvarchar](50) NULL,
    [LastName] [nvarchar](50) NULL,
    [HomeAddressId] [bigint] NULL FOREIGN KEY REFERENCES [dbo].[Addresses]([Id])
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[Customers]'
GO
CREATE TABLE [dbo].[Customers](
    [PersonId] [bigint] NOT NULL PRIMARY KEY REFERENCES [dbo].[People]([Id]),
    [CustomerStatus] [int] NOT NULL,
    [DefaultShipmentAddressId] [bigint] NULL FOREIGN KEY REFERENCES [dbo].[Addresses]([Id])
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[EmployeeRoles]'
GO
CREATE TABLE [dbo].[EmployeeRoles](
    [Id] [bigint] NOT NULL PRIMARY KEY,
    [Name] [nvarchar](50) NOT NULL,
    [Description] [nvarchar](250) NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[Employees]'
GO
CREATE TABLE [dbo].[Employees](
    [PersonId] [bigint] NOT NULL PRIMARY KEY REFERENCES [dbo].[People]([Id]),
    [RoleId] [bigint] NULL REFERENCES [dbo].[EmployeeRoles]([Id]),
    [EntryDate] [datetime] NOT NULL,
    [ManagerId] [bigint] NULL FOREIGN KEY REFERENCES [dbo].[Employees]([PersonId])
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[Demographics]'
GO
CREATE TABLE [dbo].[Demographics](
    [Id] [bigint] NOT NULL PRIMARY KEY REFERENCES [dbo].[Customers]([PersonId]),
    [Key] [nvarchar](50) NOT NULL,
    [Value] [nvarchar](50) NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[CustomerDemographics]'
GO
CREATE TABLE [dbo].[CustomerDemographics](
    [CustomerId] [bigint] NOT NULL REFERENCES [dbo].[Customers]([PersonId]),
    [DemographicId] [bigint] NOT NULL REFERENCES [dbo].[Demographics]([Id])
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CustomerDemographics] ADD PRIMARY KEY ([CustomerId],[DemographicId])
GO

 
 
PRINT '## CREATE DATA #######################################################'
GO
USE [master]
GO
ALTER DATABASE [TableInheritanceDemoDB] SET READ_WRITE 
GO
USE TableInheritanceDemoDB
GO
SET NOCOUNT ON 
GO

PRINT '   INSERT PEOPLE'
GO
INSERT INTO [dbo].[People]([FirstName],[LastName])
          SELECT 'first1', 'last1'
UNION ALL SELECT 'first2', 'last2'
UNION ALL SELECT 'first3', 'last3'
UNION ALL SELECT 'first4', 'last4'
GO

PRINT '   INSERT CUSTOMERS'
GO
INSERT INTO [dbo].[Customers]([PersonId],[CustomerStatus])
          SELECT Id, 1 FROM [dbo].[People] WHERE [LastName] = 'last1'
UNION ALL SELECT Id, 1 FROM [dbo].[People] WHERE [LastName] = 'last2'
GO

PRINT '   INSERT EMPLOYEES'
INSERT INTO [dbo].[Employees]([PersonId],[EntryDate])
          SELECT Id, '2014-01-01' FROM [dbo].[People] WHERE [LastName] = 'last3'
UNION ALL SELECT Id, '2014-05-01' FROM [dbo].[People] WHERE [LastName] = 'last4'
GO
