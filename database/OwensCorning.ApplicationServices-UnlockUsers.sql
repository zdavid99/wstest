/****** Object:  StoredProcedure [dbo].[UnlockUsers]    Script Date: 03/26/2010 19:08:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
-- =============================================
-- Author:		Jonathan Kade
-- Create date: 3/26/2010
-- Description:	Unlocks users that have been locked due to too many login attempts.

To test script:
EXEC	[dbo].[UnlockUsers]
-- =============================================
*/
CREATE PROCEDURE [dbo].[UnlockUsers]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	IF (SELECT CURSOR_STATUS('global','curLocked')) <0 
	BEGIN
		DECLARE @LockedUserName nvarchar(256)	

		DECLARE curLocked CURSOR LOCAL FAST_FORWARD FOR
		SELECT UserName 
		FROM aspnet_membership am, aspnet_users au
		where am.UserId = au.UserId
		and islockedout = 1 
		and datediff(minute,lastlockoutdate,getdate()) >= 1
		
		OPEN curLocked
		
		FETCH NEXT FROM curLocked INTO @LockedUserName
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC aspnet_Membership_UnlockUser @ApplicationName='OwensCorning', @UserName=@LockedUserName
		
			FETCH NEXT FROM curLocked INTO @LockedUserName
		END
	END
	
	IF (SELECT CURSOR_STATUS('global','curLocked')) >=0 
	BEGIN
		CLOSE curLocked
		DEALLOCATE curLocked
	END
END
