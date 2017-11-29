-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James Myers
-- Create date: 2010-07-08
-- Description:	Returns a single string property value
-- =============================================
CREATE FUNCTION dbo.GetUserProfileStringValue
(
	@ApplicationName NVARCHAR(256),
	@UserId UNIQUEIDENTIFIER,
	@PropertyName NVARCHAR(50)
)
RETURNS NVARCHAR(50)
AS
BEGIN

    DECLARE @PropertyValue NVARCHAR(50)

    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN NULL

	SELECT
		@PropertyValue = SUBSTRING(PropertyValuesString, CAST(SUBSTRING(PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3) - (CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)) AS INT)+1, CAST(SUBSTRING(PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1, CHARINDEX(':', PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1)-(CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1) ) AS INT))
	FROM aspnet_Profile P
		JOIN aspnet_Users U ON U.UserId = P.UserId
	WHERE U.ApplicationId = @ApplicationId
	AND U.UserId = @UserId
	-- WHERE THE PROPERTY EXISTS
	AND PropertyNames LIKE '%' + @PropertyName + '%'
	-- AND IS NOT EMPTY
	AND SUBSTRING(PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3, 4) != '0:-1'

	RETURN @PropertyValue
	
END
GO

