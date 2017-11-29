IF NOT EXISTS(SELECT TOP 1 1 FROM syscolumns WHERE id = OBJECT_ID('ContactListNameMap') AND name = 'ColumnMap')
BEGIN

	ALTER TABLE ContactListNameMap ADD ColumnMap VARCHAR(4000) NULL

END
GO

DECLARE @columnMap VARCHAR(4000)
SET @columnMap = 'PSEName["Lowe''s PS-E Name":1]PSEContactInfo["Lowe''s PS-E Phone Number":2]LowesStoreNumber["Lowe''s Store Number":3]LowesPurchaseOrderNumber["Lowe''s Purchase Order Number":4]InstallerVendorNumber["Installer Company Vendor Number":5]Company["Installer Company Name":6]FirstName["Installer First Name":7]LastName["Installer Last Name":8]AddressLine1["Installer Company Address":9]AddressLine2["Installer Company Address 2":10]City["Installer City":11]State["Installer State/Province":12]Zip["Installer ZIP/Postal Code":13]Email["Installer E-Mail Address":14]CustomerPropertyType["Property Type":15]CustomerPropertyTypeOther["Property Type - Other":16]CustomerName["Property Owner''s Name":17]CustomerAddressLine1["Installation Address":18]CustomerAddressLine2["Installation Address 2":19]CustomerCity["Installation City":20]CustomerState["Installation State/Province":21]CustomerZip["Installation ZIP/Postal Code":22]CustomerPropertyBuiltYear["Year Property Was Built":23]PropertyOwnerName["Contact Name":24]PropertyOwnerAddressLine1["Property Owner Mailing Address":25]PropertyOwnerAddressLine2["Property Owner Mailing Address 2":26]PropertyOwnerCity["Property Owner City":27]PropertyOwnerState["Property Owner State/Province":28]PropertyOwnerZip["Property Owner ZIP/Postal Code":29]PropertyOwnerDaytimePhone["Property Owner Phone (Daytime)":30]PropertyOwnerEveningPhone["Property Owner Phone (Evening)":31]PropertyOwnerEmail["Property Owner E-Mail Address":32]ProductPurchasedFrom["Product Purchased From":33]RoofType["Roof Type":34]ShingleTypeInstalled["Shingles Installed":35]ShingleColor["Shingle Color":36]AlgaeResistant["Algae Resistant":37]NumberOfShingleSquares["Number of squares":38]VentilationProduct["Ventilation Products":39]VentilationProductOther["Ventilation Products - Other":40]HipAndRidgeProduct["Hip and Ridge Shingles":41]HipAndRidgeProductOther["Hip and Ridge Shingles - Other":42]IceAndWaterProtectionProduct["Ice and Water Protection":43]IceAndWaterProtectionProductOther["Ice and Water Protection - Other":44]StarterShingleProduct["Starter Shingle Products":45]StarterShingleProductOther["Starter Shingle Products - Other":46]UnderlaymentProduct["Underlayment Products":47]UnderlaymentProductOther["Underlayment Products - Other":48]AdditionalProducts["Additional Products":49]Q1AllPreExistingItemsRemoved["Q1: Were all pre-existing roof materials removed down to the deck? For all new construction, check yes":50]Q2IceAndWaterProtectedAppliedToCode["Q2: Was Ice and Water Protection applied to satisfy code requirements?":51]Q3SufficientUnderlaymentUsed["Q3: Was Owens Corning™ Fiberglas™ Reinforced Felt Underlayment or #15 or #30 felt underlayment meeting ASTM D226 or ASTM D4869 used? (If synthetic underlayment was used instead of felt, check yes)":52]Q4MetalFlashingsAndDripEdgesInstalledToGuidelines["Q4: Were new metal flashings and drip edges installed per Owens Corning roofing recommendations and NRCA guidelines?":53]Q5RoofInstalledToOCGuidelinesAndLocalCode["Q5: Was the roof installed as per the requirements published by Owens Corning installation instructions in conformance with good roofing practices, and were local building code requirements observed?":54]Q6FHAMinimumVentilationUsed["Q6: Were ventilation products used to meet FHA Minimum Property ventilation standards?":55]DateCreated["Submission Date":56]'

IF EXISTS(SELECT TOP 1 1 FROM ContactListNameMap WHERE SourceFormName = 'PSE Warranty Registration' AND BusinessName = 'Lowes/OC')
BEGIN
	UPDATE ContactListNameMap SET
		ColumnMap = @columnMap
	WHERE SourceFormName = 'PSE Warranty Registration' AND BusinessName = 'Lowes/OC'
END
ELSE
BEGIN
	INSERT INTO ContactListNameMap(SourceFormName,Name,Active,BusinessName,ColumnMap)
	VALUES('PSE Warranty Registration','PSE Warranty Registration',1,'Lowes/OC',@columnMap)
END

select * from ContactListNameMap WHERE SourceFormName = 'PSE Warranty Registration' AND BusinessName = 'Lowes/OC'
