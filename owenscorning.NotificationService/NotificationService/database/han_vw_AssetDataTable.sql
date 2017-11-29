
/****** Object:  View [dbo].[ecom_AddressListView]    Script Date: 04/09/2010 09:51:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[han_vw_AssetDataTable]
AS
SELECT cast([id] AS varchar(50)) as [id]
      ,[handle]
      ,[name]
      ,[mimeType]
      ,[storage]
      ,[length]
      ,[version]
      ,[status]
      ,[dateModified]
      ,[label]
      ,[astLanguage]
      ,[assetType]
      ,[publishName]
      ,[serverId]
      ,[publishAsPDF]
      ,[pubFolderPath]
  FROM [dbo].[AssetDataTable]

GO


