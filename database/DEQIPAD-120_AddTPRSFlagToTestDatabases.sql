
alter table [dbo].[dealers] add Total_Protection_Rfg_System varchar(1) null
GO

ALTER TABLE [dbo].[dealers] ADD  CONSTRAINT [DF_dealers_Total_Protection_Rfg_System]  DEFAULT ('N') FOR [Total_Protection_Rfg_System]
GO

UPDATE dbo.dealers
SET [Total_Protection_Rfg_System] = CASE WHEN id%2=0 THEN 'Y' ELSE 'N' END

alter table [dbo].[dealers] alter column Total_Protection_Rfg_System varchar(1) not null
GO

