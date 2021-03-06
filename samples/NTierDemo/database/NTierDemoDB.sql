USE [master]
GO

PRINT '## DROP DATABASE #######################################################'
GO
IF DB_ID (N'NTierDemoDB_Jun2014') IS NOT NULL
BEGIN
    PRINT '   DROP DATABASE [NTierDemoDB_Jun2014]'
    EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'NTierDemoDB_Jun2014'
    ALTER DATABASE [NTierDemoDB_Jun2014] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [NTierDemoDB_Jun2014];
END

PRINT '## CREATE DATABASE #######################################################'
GO
DECLARE @sql NVARCHAR(1024), @path VARCHAR(256)

SELECT @path = PHYSICAL_NAME FROM sys.master_files WHERE database_id = DB_ID(N'master') AND TYPE_DESC = 'ROWS'
SET @path = REVERSE(RIGHT(REVERSE(@path),(LEN(@path)-CHARINDEX('\', REVERSE(@path),1))+1))

PRINT '   CREATE DATABASE [NTierDemoDB_Jun2014]'
PRINT '   '+@path+'NTierDemoDB_Jun2014.mdf'
PRINT '   '+@path+'NTierDemoDB_Jun2014_log.ldf'

SET @sql = 
N'CREATE DATABASE [NTierDemoDB_Jun2014] 
  CONTAINMENT = NONE 
  ON  PRIMARY 
  ( NAME = N''NTierDemoDB_Jun2014'', FILENAME = N'''+@path+N'NTierDemoDB_Jun2014.mdf'' , SIZE = 3136KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) 
  LOG ON 
  ( NAME = N''NTierDemoDB_Jun2014_Log'', FILENAME = N'''+@path+N'NTierDemoDB_Jun2014_log.ldf'' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
EXEC sp_executesql @sql

ALTER DATABASE [NTierDemoDB_Jun2014] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NTierDemoDB_Jun2014].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET ARITHABORT OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET  DISABLE_BROKER 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET  MULTI_USER 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [NTierDemoDB_Jun2014]
GO


PRINT '## CREATE SCHEMA #######################################################'
GO
PRINT '   CREATE SCHEMA [pub]'
GO
CREATE SCHEMA [pub]
GO


PRINT '## CREATE TABLES #######################################################'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

PRINT '   CREATE TABLE [pub].[User]'
GO
CREATE TABLE [pub].[User](
    [Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username] [varchar](30) NOT NULL,
    [Password] [varchar](30) NOT NULL,
    [FirstName] [varchar](50) NOT NULL,
    [LastName] [varchar](50) NOT NULL,
    [Description] [varchar](500) NULL,
    [CreatedDate] [datetime2] NOT NULL,
    [ModifiedDate] [datetime2] NOT NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [pub].[Blog]'
GO
CREATE TABLE [pub].[Blog](
    [Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OwnerId] [bigint] NOT NULL FOREIGN KEY REFERENCES [pub].[User]([Id]),
    [Title] [varchar](100) NOT NULL,
    [Description] [varchar](500) NULL,
    [CreatedDate] [datetime2] NOT NULL,
    [ModifiedDate] [datetime2] NOT NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [pub].[Post]'
GO
CREATE TABLE [pub].[Post](
    [Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BlogId] [bigint] NOT NULL FOREIGN KEY REFERENCES [pub].[Blog]([Id]),
    [Title] [varchar](100) NOT NULL,
    [Abstract] [varchar](200) NOT NULL,
    [Content] [varchar](max) NOT NULL,
    [CreatedDate] [datetime2] NOT NULL,
    [ModifiedDate] [datetime2] NOT NULL
) ON [PRIMARY]
GO
 
 
PRINT '## CREATE DATA #######################################################'
GO
USE [master]
GO
ALTER DATABASE [NTierDemoDB_Jun2014] SET READ_WRITE 
GO
USE NTierDemoDB_Jun2014
GO
SET NOCOUNT ON 
GO

PRINT '   INSERT USERS'
GO
INSERT INTO [pub].[User]([Username], [Password], [FirstName], [LastName], [Description], [CreatedDate], [ModifiedDate])
          SELECT 'thuber', '****', 'Thomas', 'Huber', 'MVP Client Development', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
UNION ALL SELECT 'nmueggler', '****', 'Nicolas', 'Mueggler', 'TFS Guru', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
UNION ALL SELECT 'anobbmann', '****', 'Andreas', 'Nobbmann', 'All about ORACLE BI', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
UNION ALL SELECT 'gschmutz', '****', 'Guido', 'Schmutz', 'Knows everything about SOA with Oracle', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
UNION ALL SELECT 'mmeyer', '****', 'Manuel', 'Meyer', '.NET, Azure, everything', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
GO

PRINT '   INSERT BLOGS'
GO
INSERT INTO [pub].[Blog]([Title], [Description], [CreatedDate], [ModifiedDate], [OwnerId])
          SELECT '.NET Development, Performance Management and the Windows Azure Cloud', NULL, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, (SELECT TOP 1 [Id] FROM [pub].[User] WHERE [Username] = 'mmeyer')
UNION ALL SELECT 'User Interface Rocker', NULL, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, (SELECT TOP 1 [Id] FROM [pub].[User] WHERE [Username] = 'thuber')
GO

PRINT '   INSERT BLOG POSTS'
INSERT INTO [pub].[Post]([Title], [CreatedDate], [ModifiedDate], [BlogId], [Abstract], [Content])
          SELECT 'Get started with the Azure Cloud!', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, (SELECT TOP 1 [Id] FROM [pub].[Blog] WHERE [OwnerId] = (SELECT TOP 1 [Id] FROM [pub].[User] WHERE [Username] = 'mmeyer')), 'Get started with Azure using Azure Friday, a collection of bite-size (10-15mins) videos on Windows Azure...', '...'
UNION ALL SELECT 'Next Generation Windows Apps', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, (SELECT TOP 1 [Id] FROM [pub].[Blog] WHERE [OwnerId] = (SELECT TOP 1 [Id] FROM [pub].[User] WHERE [Username] = 'thuber')), '....', '...'
GO