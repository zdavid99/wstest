-- Create mapping table. . .
CREATE TABLE [dbo].[ContactListNameMap](
	[SourceFormName]  nvarchar(255) NOT NULL,
	[Name] nvarchar(255) NOT NULL,
	[Active] bit NOT NULL,
 CONSTRAINT [PK_ContactListNameMap] PRIMARY KEY CLUSTERED 
(
	[SourceFormName] ASC
) ON [PRIMARY]
) ON [PRIMARY]




BEGIN TRANSACTION 

DELETE FROM [ContactListNameMap]

  INSERT INTO [ContactListNameMap]
 SELECT DISTINCT [source_form_name], [source_form_name], 1
  FROM [oc_contactlist]


COMMIT TRANSACTION 




BEGIN TRANSACTION

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'BFS Request Info Form'

UPDATE [ContactListNameMap]
SET Name = 'OC ''Roofing'''
WHERE SourceFormName = 'Contractor Rewards Form'

UPDATE [ContactListNameMap]
SET Name = 'Franchising' 
WHERE SourceFormName = 'Sunroom Request Info Form'

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'Franchising Request Info Form'

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'SunSuites V Request Info Form'

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'Solace Request Info Form'

UPDATE [ContactListNameMap]
SET Name = 'Roofing'
WHERE SourceFormName = 'Shingle Recycling Registration'

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'Innovision Request Info Form'

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'Solace Promo Form'

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'VClass Promo Form'

UPDATE [ContactListNameMap]
SET Name = 'Franchising'
WHERE SourceFormName = 'VClass Request Info Form'

UPDATE [ContactListNameMap]
SET Name = 'Cultured Stone'
WHERE SourceFormName = 'StoneCAD® Registration'


COMMIT TRANSACTION 


  