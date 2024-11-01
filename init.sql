USE [KiloMart]
GO
/****** Object:  Table [dbo].[Language]    Script Date: 11/2/2024 2:35:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Language](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderStatus] [tinyint] NOT NULL,
	[TotalPrice] [money] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItem]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [bigint] NOT NULL,
	[ProductOffer] [int] NOT NULL,
	[Price] [money] NOT NULL,
 CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatus]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatus](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageUrl] [varchar](300) NOT NULL,
	[ProductCategory] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProductCategory_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategoryLocalized]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategoryLocalized](
	[Name] [varchar](200) NOT NULL,
	[Language] [tinyint] NOT NULL,
	[ProductCategory] [int] NOT NULL,
 CONSTRAINT [PK_ProductCategoryLocalized] PRIMARY KEY CLUSTERED 
(
	[Language] ASC,
	[ProductCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductLocalized]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductLocalized](
	[Language] [tinyint] NOT NULL,
	[Product] [int] NOT NULL,
	[MeasurementUnit] [varchar](200) NOT NULL,
	[Description] [varchar](300) NOT NULL,
	[Name] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ProductLocalized] PRIMARY KEY CLUSTERED 
(
	[Language] ASC,
	[Product] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductOffer]    Script Date: 11/2/2024 2:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductOffer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Product] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[OffPercentage] [decimal](10, 5) NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NULL,
	[Quantity] [decimal](10, 5) NOT NULL,
	[Provider] [int] NOT NULL,
 CONSTRAINT [PK_ProductOffer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_OrderStatus] FOREIGN KEY([OrderStatus])
REFERENCES [dbo].[OrderStatus] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_OrderStatus]
GO
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Order]
GO
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_ProductOffer] FOREIGN KEY([ProductOffer])
REFERENCES [dbo].[ProductOffer] ([Id])
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_ProductOffer]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductCategory] FOREIGN KEY([ProductCategory])
REFERENCES [dbo].[ProductCategory] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductCategory]
GO
ALTER TABLE [dbo].[ProductCategoryLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategoryLocalized_Language1] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[ProductCategoryLocalized] CHECK CONSTRAINT [FK_ProductCategoryLocalized_Language1]
GO
ALTER TABLE [dbo].[ProductCategoryLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategoryLocalized_ProductCategory1] FOREIGN KEY([ProductCategory])
REFERENCES [dbo].[ProductCategory] ([Id])
GO
ALTER TABLE [dbo].[ProductCategoryLocalized] CHECK CONSTRAINT [FK_ProductCategoryLocalized_ProductCategory1]
GO
ALTER TABLE [dbo].[ProductLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProductLocalized_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[ProductLocalized] CHECK CONSTRAINT [FK_ProductLocalized_Language]
GO
ALTER TABLE [dbo].[ProductLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProductLocalized_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductLocalized] CHECK CONSTRAINT [FK_ProductLocalized_Product]
GO
ALTER TABLE [dbo].[ProductOffer]  WITH CHECK ADD  CONSTRAINT [FK_ProductOffer_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductOffer] CHECK CONSTRAINT [FK_ProductOffer_Product]
GO
USE [master]
GO
ALTER DATABASE [KiloMart] SET  READ_WRITE 
GO
