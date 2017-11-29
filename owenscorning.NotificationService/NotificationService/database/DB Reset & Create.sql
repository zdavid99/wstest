USE [OC_NotificationService]
GO

/********** DROP STUFF ********/


/****** Object:  Table [dbo].[SiteConfiguration]    Script Date: 08/04/2009 15:37:30 ******/

/****** Object:  Table [dbo].[SiteConfiguration]    Script Date: 08/04/2009 15:39:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SiteConfiguration]') AND type in (N'U'))
DROP TABLE [dbo].[SiteConfiguration]
GO




IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UpdatedDocuments_Batch]') AND parent_object_id = OBJECT_ID(N'[dbo].[UpdatedDocuments]'))
ALTER TABLE [dbo].[UpdatedDocuments] DROP CONSTRAINT [FK_UpdatedDocuments_Batch]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SentInfo_Batch]') AND parent_object_id = OBJECT_ID(N'[dbo].[SentInfo]'))
ALTER TABLE [dbo].[SentInfo] DROP CONSTRAINT [FK_SentInfo_Batch]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SentInfo_Subscription]') AND parent_object_id = OBJECT_ID(N'[dbo].[SentInfo]'))
ALTER TABLE [dbo].[SentInfo] DROP CONSTRAINT [FK_SentInfo_Subscription]
GO

/****** Object:  Table [dbo].[UpdatedDocuments]    Script Date: 07/02/2009 13:47:25 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatedDocuments]') AND type in (N'U'))
DROP TABLE [dbo].[UpdatedDocuments]
GO

/****** Object:  Table [dbo].[Batch]    Script Date: 07/02/2009 13:48:35 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Batch]') AND type in (N'U'))
DROP TABLE [dbo].[Batch]
GO

/****** Object:  Table [dbo].[Subscription]    Script Date: 07/02/2009 13:50:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscription]') AND type in (N'U'))
DROP TABLE [dbo].[Subscription]
GO

/****** Object:  Table [dbo].[SentInfo]    Script Date: 07/07/2009 17:45:41 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SentInfo]') AND type in (N'U'))
DROP TABLE [dbo].[SentInfo]
GO

/************** CREATE STUFF ****************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

USE [OC_NotificationService]
GO

/****** Object:  Table [dbo].[SiteConfiguration]    Script Date: 08/04/2009 15:33:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE TABLE [dbo].[SiteConfiguration](
	[site] [varchar](50) NOT NULL,
	[siteName] [varchar](255) NOT NULL,
	[emailTemplateURL] [varchar](255) NULL,
	[taxonomyName] [varchar](255) NULL
) ON [PRIMARY]

GO



/****** Object:  Table [dbo].[UpdatedDocuments]    Script Date: 07/09/2009 10:33:44 ******/
CREATE TABLE [dbo].[UpdatedDocuments](
	[pk_id] [int] IDENTITY(1,1) NOT NULL,
	[documentName] [varchar](512) NOT NULL,
	[url] [varchar](1024) NOT NULL,
	[dateUpdated] [datetime] NULL,
	[fileSize] [varchar](20) NULL,
	[documentType] [varchar](50) NULL,
	[batch] [int] NULL,
 CONSTRAINT [PK_UpdatedDocuments] PRIMARY KEY CLUSTERED 
(
	[pk_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Batch]    Script Date: 07/08/2009 11:55:33 ******/
CREATE TABLE [dbo].[Batch](
	[batchId] [int] IDENTITY(1,1) NOT NULL,
	[site] [varchar](512) NOT NULL,
	[startDate] [datetime] NOT NULL,
	[endDate] [datetime] NOT NULL,
	[finishDate] [datetime] NULL,
	[status] [varchar](20) NULL,
 CONSTRAINT [PK_Batch] PRIMARY KEY CLUSTERED 
(
	[batchId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Subscription]    Script Date: 07/08/2009 11:57:28 ******/
CREATE TABLE [dbo].[Subscription](
	[email] [varchar](512) NOT NULL,
	[site] [varchar](512) NOT NULL,
	[firstName] [varchar](100) NULL,
	[lastName] [varchar](100) NULL,
	[optedIn] [bit] NOT NULL,
 CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED 
(
	[email] ASC,
	[site] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[SentInfo]    Script Date: 07/08/2009 11:56:11 ******/
CREATE TABLE [dbo].[SentInfo](
	[batchId] [int] NOT NULL,
	[subscriptionEmail] [varchar](512) NOT NULL,
	[site] [varchar](512) NOT NULL,
	[lastSendDate] [datetime] NULL,
	[status] [varchar](20) NULL,
 CONSTRAINT [PK_SentInfo] PRIMARY KEY CLUSTERED 
(
	[batchId] ASC,
	[subscriptionEmail] ASC,
	[site] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/*************** CREATE CONSTRAINTS *******************/

ALTER TABLE [dbo].[UpdatedDocuments]  WITH NOCHECK ADD  CONSTRAINT [FK_UpdatedDocuments_Batch] FOREIGN KEY([batch])
REFERENCES [dbo].[Batch] ([batchId])
GO

ALTER TABLE [dbo].[UpdatedDocuments] CHECK CONSTRAINT [FK_UpdatedDocuments_Batch]
GO

ALTER TABLE [dbo].[SentInfo]  WITH CHECK ADD  CONSTRAINT [FK_SentInfo_Batch] FOREIGN KEY([batchId])
REFERENCES [dbo].[Batch] ([batchId])
GO

ALTER TABLE [dbo].[SentInfo] CHECK CONSTRAINT [FK_SentInfo_Batch]
GO

ALTER TABLE [dbo].[SentInfo]  WITH CHECK ADD  CONSTRAINT [FK_SentInfo_Subscription] FOREIGN KEY([subscriptionEmail], [site])
REFERENCES [dbo].[Subscription] ([email], [site])
GO

ALTER TABLE [dbo].[SentInfo] CHECK CONSTRAINT [FK_SentInfo_Subscription]
GO



INSERT INTO [OC_NotificationService].[dbo].[SiteConfiguration]
           ([site]
           ,[siteName]
           ,[emailTemplateURL]
           ,[taxonomyName])
     VALUES
           ('commercial.owenscorning.com'
			,'Owens Corning Commercial Insulation'
           ,'http://commercial.owenscorning.com/email/template.aspx'
           ,'Document Categories')
GO
INSERT INTO [OC_NotificationService].[dbo].[SiteConfiguration]
           ([site]
           ,[siteName]
           ,[emailTemplateURL]
           ,[taxonomyName])
     VALUES
           ('commercial.staging.hansoninc.com'
			,'Owens Corning Commercial Insulation'
           ,'http://commercial.staging.hansoninc.com/email/template.aspx'
           ,'Document Categories')
GO


