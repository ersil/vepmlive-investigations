SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

IF OBJECT_ID(N'dbo.LICENSEPRODUCTS', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[LICENSEPRODUCTS](
		[product_id] [int] IDENTITY(1,1) NOT NULL,
		[sku] [varchar](255) NOT NULL,
		[name] [varchar](255) NOT NULL,
		[active] [bit] NOT NULL CONSTRAINT [DF_LICENSEPRODUCTS_active]  DEFAULT ((1)),
	 CONSTRAINT [PK_HOSTEDPRODUCTS] PRIMARY KEY CLUSTERED 
	(
		[product_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	 CONSTRAINT [HOSTEDPRODUCTS_UNIQUE_SKUS] UNIQUE NONCLUSTERED 
	(
		[sku] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	
END
GO

IF OBJECT_ID(N'dbo.LICENSEDETAIL', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LICENSEDETAIL](
		[license_detail_id] [int] IDENTITY(1,1) NOT NULL,
		[product_id] [int] NOT NULL,
		[detail_type_id] [int] NULL,
		[contract_id] [int] NULL,
	 CONSTRAINT [PK_PRODPLANS] PRIMARY KEY CLUSTERED 
	(
		[license_detail_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF OBJECT_ID(N'dbo.DETAILTYPES', N'U') IS NOT NULL
BEGIN
	ALTER TABLE DETAILTYPES
	ALTER COLUMN detail_type_id INTEGER NOT NULL
END
GO
IF OBJECT_ID(N'dbo.DETAILTYPES', N'U') IS NOT NULL AND  NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = 'DETAILTYPES' AND TABLE_SCHEMA ='dbo' )
BEGIN
	ALTER TABLE DETAILTYPES
	ADD CONSTRAINT PK_DETAILTYPES PRIMARY KEY (detail_type_id)
END
GO


IF OBJECT_ID(N'dbo.LICENSEDETAIL', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_LICENSEDETAIL_DETAILTYPES' AND TABLE_NAME = 'LICENSEDETAIL' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[LICENSEDETAIL]  WITH CHECK ADD  CONSTRAINT [FK_LICENSEDETAIL_DETAILTYPES] FOREIGN KEY([detail_type_id])
	REFERENCES [dbo].[DETAILTYPES] ([detail_type_id])
END
GO

IF OBJECT_ID(N'dbo.LICENSEDETAIL', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_LICENSEDETAIL_DETAILTYPES' AND TABLE_NAME = 'LICENSEDETAIL' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[LICENSEDETAIL] CHECK CONSTRAINT [FK_LICENSEDETAIL_DETAILTYPES]
END
GO

IF OBJECT_ID(N'dbo.LICENSEDETAIL', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_PRODPLANS_HOSTEDPRODUCTS' AND TABLE_NAME = 'LICENSEDETAIL' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[LICENSEDETAIL]  WITH CHECK ADD  CONSTRAINT [FK_PRODPLANS_HOSTEDPRODUCTS] FOREIGN KEY([product_id])
	REFERENCES [dbo].[LICENSEPRODUCTS] ([product_id])
END
GO

IF OBJECT_ID(N'dbo.LICENSEDETAIL', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_PRODPLANS_HOSTEDPRODUCTS' AND TABLE_NAME = 'LICENSEDETAIL' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[LICENSEDETAIL] CHECK CONSTRAINT [FK_PRODPLANS_HOSTEDPRODUCTS]
END
GO


IF OBJECT_ID(N'dbo.ORDERS', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = N'product_id' AND TABLE_NAME = N'ORDERS')
BEGIN
	ALTER TABLE ORDERS
	ADD product_id INTEGER NULL
END
GO

IF OBJECT_ID(N'dbo.ORDERS', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_ORDERS_LICENSEPRODUCTS' AND TABLE_NAME = 'ORDERS' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[ORDERS]  WITH CHECK ADD  CONSTRAINT [FK_ORDERS_LICENSEPRODUCTS] FOREIGN KEY([product_id])
	REFERENCES [dbo].[LICENSEPRODUCTS] ([product_id])
END
GO

IF OBJECT_ID(N'dbo.ORDERS', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_ORDERS_LICENSEPRODUCTS' AND TABLE_NAME = 'ORDERS' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[ORDERS] CHECK CONSTRAINT [FK_ORDERS_LICENSEPRODUCTS]
END
GO


IF OBJECT_ID(N'dbo.CONTRACTLEVEL_TITLES', N'U') IS NOT NULL
BEGIN
	ALTER TABLE CONTRACTLEVEL_TITLES
	ALTER COLUMN CONTRACTLEVEL INTEGER NOT NULL
END
GO
IF OBJECT_ID(N'dbo.CONTRACTLEVEL_TITLES', N'U') IS NOT NULL AND  NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = 'CONTRACTLEVEL_TITLES' AND TABLE_SCHEMA ='dbo' )
BEGIN
	ALTER TABLE CONTRACTLEVEL_TITLES
	ADD CONSTRAINT PK_CONTRACTLEVEL_TITLES PRIMARY KEY (CONTRACTLEVEL)
END
GO


IF OBJECT_ID(N'dbo.CONTRACTLEVELS', N'U') IS NOT NULL AND EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = N'CONTRACTLEVELID' AND TABLE_NAME = N'CONTRACTLEVELS')
BEGIN
	ALTER TABLE CONTRACTLEVELS
	ALTER COLUMN CONTRACTLEVELID uniqueidentifier NOT NULL
END
GO
IF OBJECT_ID(N'dbo.CONTRACTLEVELS', N'U') IS NOT NULL AND  NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = 'CONTRACTLEVELS' AND TABLE_SCHEMA ='dbo' )
BEGIN
	ALTER TABLE CONTRACTLEVELS
	ADD CONSTRAINT PK_CONTRACTLEVELS PRIMARY KEY (CONTRACTLEVELID)
END
GO
IF OBJECT_ID(N'dbo.CONTRACTLEVELS', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_CONTRACTLEVELS_CONTRACTLEVEL_TITLES' AND TABLE_NAME = 'CONTRACTLEVELS' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[CONTRACTLEVELS]  WITH CHECK ADD  CONSTRAINT [FK_CONTRACTLEVELS_CONTRACTLEVEL_TITLES] FOREIGN KEY([contractlevel])
	REFERENCES [dbo].[CONTRACTLEVEL_TITLES] ([contractlevel])
END
GO

IF OBJECT_ID(N'dbo.CONTRACTLEVELS', N'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_CONTRACTLEVELS_CONTRACTLEVEL_TITLES' AND TABLE_NAME = 'CONTRACTLEVELS' AND TABLE_SCHEMA ='dbo')
BEGIN
	ALTER TABLE [dbo].[CONTRACTLEVELS] CHECK CONSTRAINT [FK_CONTRACTLEVELS_CONTRACTLEVEL_TITLES]
END
GO

SET ANSI_PADDING OFF
GO