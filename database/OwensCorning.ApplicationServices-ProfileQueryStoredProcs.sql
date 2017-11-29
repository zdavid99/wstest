USE [OCCommon]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetUsersWithProfileValue]    Script Date: 05/18/2010 10:09:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- ==================================================================
-- Author:		James Myers
-- Create date:	5.18.2010
-- Description:	Basic query over string valued properties.  Returns user ids matching the criteria
-- ==================================================================
CREATE PROCEDURE [dbo].[usp_GetUsersWithProfileValue]
	@ApplicationName NVARCHAR(256),
	@PropertyName NVARCHAR(50),
	@QueryValue NVARCHAR(50)
AS
BEGIN

    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

	SELECT
		U.UserId
		--SUBSTRING(PropertyValuesString, CAST(SUBSTRING(PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3) - (CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)) AS INT)+1, CAST(SUBSTRING(PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1, CHARINDEX(':', PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1)-(CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1) ) AS INT)) value,
		--Start = SUBSTRING(PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3) - (CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)),
		--[End] = SUBSTRING(PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1, CHARINDEX(':', PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1)-(CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1) )
	FROM aspnet_Profile P
		JOIN aspnet_Users U ON U.UserId = P.UserId
	WHERE U.ApplicationId = @ApplicationId
	-- WHERE THE PROPERTY EXISTS
	AND PropertyNames LIKE '%' + @PropertyName + '%'
	-- AND IS NOT EMPTY
	AND SUBSTRING(PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3, 4) != '0:-1'
	-- AND SUBSTRING MATCHES (THIS IS QUITE A STRING!)
	AND SUBSTRING(PropertyValuesString, CAST(SUBSTRING(PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3) - (CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)) AS INT)+1, CAST(SUBSTRING(PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1, CHARINDEX(':', PropertyNames, CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1)-(CHARINDEX(':', PropertyNames, CHARINDEX(@PropertyName, PropertyNames)+LEN(@PropertyName)+3)+1) ) AS INT)) = @QueryValue

	
END

GO

