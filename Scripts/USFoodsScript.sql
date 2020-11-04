
IF OBJECT_ID('Purchnge') IS NOT NULL
DROP TABLE Purchnge

CREATE TABLE [dbo].[Purchnge]
(
[Supplier Name] [varchar] (20) NULL,
[Delivery Note No.] [varchar] (15) NULL
)

IF OBJECT_ID('usfFieldDef') IS NOT NULL
DROP TABLE usfFieldDef

CREATE TABLE [dbo].[usfFieldDef]
(
[FieldID] [smallint] NOT NULL,
[FieldName] [nvarchar] (30) NOT NULL,
[StartChar] [smallint] NULL,
[Chars] [smallint] NULL,
[EndChar] [smallint] NULL,
[DataType] [varchar] (20) NULL,
[PadChar] [varchar] (1) NULL,
[LeftPad] [varchar] (1) NULL
)

DELETE [dbo].[usfFieldDef]
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(1,'InvoiceNumber',11,6,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(2,'InvoiceDate',17,6,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(3,'ProductName',113,30,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(4,'UnitsQuantity',69,4,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(5,'EachesQuantity',73,3,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(6,'PriceUnitMeasure',88,2,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(7,'UnitCost',83,5,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(8,'TotalCost',105,8,null,null,null,null)
INSERT INTO [Aztec].[dbo].[usfFieldDef]([FieldID],[FieldName],[StartChar],[Chars],[EndChar],[DataType],[PadChar],[LeftPad])
     VALUES(9,'ImportReference',52,7,null,null,null,null)