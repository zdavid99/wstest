SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[UserLoginHistory]
(
	[UserLoginHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[SiteName] [nvarchar](256) NOT NULL,
	[LoginDateTime] [datetime] NOT NULL,
	CONSTRAINT [PK_UserLoginHistory] PRIMARY KEY CLUSTERED 
	(
		[UserLoginHistoryID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[UserLoginHistory] ADD  CONSTRAINT [DF_UserLoginHistory_LoginDateTime]  DEFAULT (getdate()) FOR [LoginDateTime]
GO

-- ==================================================================
-- Author:		Matt Hamilton
-- Create date:	2.17.2010
-- Description:	Insert a record in the UserLoginHistory table
-- ==================================================================
CREATE PROCEDURE usp_InsertUserLoginHistory
	@ApplicationId uniqueidentifier,
	@UserId uniqueidentifier,
	@SiteName nvarchar(256),
	@UserLoginHistoryID int output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO UserLoginHistory
	(
		ApplicationId,
		UserId,
		SiteName
	)
	VALUES
	(
		@ApplicationId,
		@UserId,
		@SiteName
	);
	
	SET @UserLoginHistoryID = SCOPE_IDENTITY();
END
GO

-- ==================================================================
-- Author:		Matt Hamilton
-- Create date:	2.17.2010
-- Description:	Retrieve an ApplicationId given an Application Name
-- ==================================================================
CREATE PROCEDURE usp_GetApplicationIdByName
	@ApplicationName nvarchar(256),
	@ApplicationId uniqueidentifier output
AS
BEGIN
	SET NOCOUNT ON;

	SELECT @ApplicationId = ApplicationId
	FROM aspnet_Applications
	WHERE lower(@ApplicationName) = LoweredApplicationName;
END
GO
