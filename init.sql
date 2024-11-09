USE [KiloMartMasterDb]
GO
/****** Object:  Table [dbo].[Card]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Card](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HolderName] [varchar](100) NOT NULL,
	[Number] [varchar](100) NOT NULL,
	[SecurityCode] [varchar](100) NOT NULL,
	[ExpireDate] [date] NOT NULL,
	[Customer] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Card] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountryLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryLocalized](
	[Country] [int] NOT NULL,
	[Language] [tinyint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CountryLocalized] PRIMARY KEY CLUSTERED 
(
	[Country] ASC,
	[Language] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[Party] [int] NOT NULL,
 CONSTRAINT [PK_Customer_1] PRIMARY KEY CLUSTERED 
(
	[Party] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerProfile]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Customer] [int] NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[SecondName] [varchar](200) NOT NULL,
	[NationalName] [varchar](200) NOT NULL,
	[NationalId] [varchar](200) NOT NULL,
 CONSTRAINT [PK_CustomerProfile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerProfileLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerProfileLocalized](
	[CustomerProfile] [int] NOT NULL,
	[Language] [tinyint] NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[SecondName] [varchar](200) NOT NULL,
	[NationalName] [varchar](200) NOT NULL,
 CONSTRAINT [PK_CustomerProfileLocalized] PRIMARY KEY CLUSTERED 
(
	[CustomerProfile] ASC,
	[Language] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DelivaryDocument]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DelivaryDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Url] [varchar](200) NOT NULL,
	[Delivary] [int] NOT NULL,
	[DocumentType] [tinyint] NOT NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DelivaryProfile]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DelivaryProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Delivary] [int] NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[SecondName] [varchar](200) NOT NULL,
	[NationalName] [varchar](200) NOT NULL,
	[NationalId] [varchar](200) NOT NULL,
	[LicenseNumber] [varchar](200) NOT NULL,
	[LicenseExpiredDate] [date] NOT NULL,
	[DrivingLicenseNumber] [varchar](200) NOT NULL,
	[DrivingLicenseExpiredDate] [date] NOT NULL,
 CONSTRAINT [PK_DelivaryProfile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DelivaryProfileLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DelivaryProfileLocalized](
	[DelivaryProfile] [int] IDENTITY(1,1) NOT NULL,
	[Language] [tinyint] NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[SecondName] [varchar](200) NOT NULL,
	[NationalName] [varchar](200) NOT NULL,
 CONSTRAINT [PK_DelivaryProfileLocalized] PRIMARY KEY CLUSTERED 
(
	[DelivaryProfile] ASC,
	[Language] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Delivery]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Delivery](
	[Party] [int] NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Party] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiscountCode]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscountCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](200) NOT NULL,
	[Value] [decimal](18, 5) NOT NULL,
	[Description] [varchar](200) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[DiscountType] [tinyint] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DiscountCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiscountType]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscountType](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DiscountType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentType](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DocumentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Language]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Language](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](10) NOT NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Location]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Location](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Longitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Party] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationDetails]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuildingType] [varchar](50) NOT NULL,
	[BuildingNumber] [varchar](50) NOT NULL,
	[FloorNumber] [varchar](50) NOT NULL,
	[ApartmentNumber] [varchar](50) NOT NULL,
	[StreetNumber] [varchar](50) NOT NULL,
	[PhoneNumber] [varchar](50) NOT NULL,
	[Location] [int] NOT NULL,
 CONSTRAINT [PK_LocationDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MembershipUser]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MembershipUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](256) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [varchar](max) NOT NULL,
	[Role] [tinyint] NOT NULL,
	[Party] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderStatus] [tinyint] NOT NULL,
	[TotalPrice] [money] NOT NULL,
	[TransactionId] [varchar](50) NOT NULL,
	[CustomerLocation] [int] NOT NULL,
	[ProviderLocation] [int] NOT NULL,
	[Customer] [int] NOT NULL,
	[Provider] [int] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderActivity]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderActivity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [bigint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[OrderActivityType] [tinyint] NOT NULL,
	[OperatedBy] [int] NOT NULL,
 CONSTRAINT [PK_OrderActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderActivityType]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderActivityType](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_OrderActivityType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDiscountCode]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDiscountCode](
	[Order] [bigint] NOT NULL,
	[DiscountCode] [int] NOT NULL,
 CONSTRAINT [PK_OrderDiscountCode] PRIMARY KEY CLUSTERED 
(
	[Order] ASC,
	[DiscountCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItem]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [bigint] NOT NULL,
	[ProductOffer] [int] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Quantity] [float] NOT NULL,
 CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItemDiscountCode]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItemDiscountCode](
	[OrderItem] [bigint] NOT NULL,
	[DiscountCode] [int] NOT NULL,
 CONSTRAINT [PK_OrderItemDiscountCode] PRIMARY KEY CLUSTERED 
(
	[OrderItem] ASC,
	[DiscountCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatus]    Script Date: 11/10/2024 2:21:56 AM ******/
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
/****** Object:  Table [dbo].[Party]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Party](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DisplayName] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Party] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartyLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartyLocalized](
	[Party] [int] NOT NULL,
	[Language] [tinyint] NOT NULL,
	[DisplayName] [varchar](200) NOT NULL,
 CONSTRAINT [PK_PartyLocalized] PRIMARY KEY CLUSTERED 
(
	[Party] ASC,
	[Language] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhoneNumber]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhoneNumber](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](50) NOT NULL,
	[Party] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PhoneNumber] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageUrl] [varchar](300) NOT NULL,
	[ProductCategory] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[MeasurementUnit] [varchar](200) NOT NULL,
	[Description] [varchar](300) NOT NULL,
	[Name] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Name] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ProductCategory_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategoryLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
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
/****** Object:  Table [dbo].[ProductDiscount]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductDiscount](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [int] NOT NULL,
	[DiscountCode] [int] NOT NULL,
	[AssignedDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProductDiscount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
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
/****** Object:  Table [dbo].[ProductOffer]    Script Date: 11/10/2024 2:21:56 AM ******/
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
	[Quantity] [float] NOT NULL,
	[Provider] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProductOffer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductOfferDiscount]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductOfferDiscount](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductOffer] [int] NOT NULL,
	[DiscountCode] [int] NOT NULL,
	[AssignedDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProductOfferDiscount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductRequest]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Provider] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[ImageUrl] [varchar](300) NOT NULL,
	[ProductCategory] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[OffPercentage] [decimal](10, 5) NOT NULL,
	[Quantity] [float] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_ProductRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductRequestDataLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductRequestDataLocalized](
	[ProductRequest] [int] NOT NULL,
	[Language] [tinyint] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Description] [varchar](300) NOT NULL,
	[MeasurementUnit] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ProductRequestDataLocalized_1] PRIMARY KEY CLUSTERED 
(
	[ProductRequest] ASC,
	[Language] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductRequestStatus]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductRequestStatus](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ProductRequestStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Provider]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Provider](
	[Party] [int] NOT NULL,
 CONSTRAINT [PK_Provider] PRIMARY KEY CLUSTERED 
(
	[Party] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProviderDocument]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[DocumentType] [tinyint] NOT NULL,
	[Url] [varchar](200) NOT NULL,
	[Provider] [int] NOT NULL,
 CONSTRAINT [PK_ProviderDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProviderProfile]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Provider] [int] NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[SecondName] [varchar](200) NOT NULL,
	[OwnerNationalId] [varchar](200) NOT NULL,
	[NationalApprovalId] [varchar](200) NOT NULL,
	[CompanyName] [varchar](200) NOT NULL,
	[OwnerName] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ProviderProfile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProviderProfileLocalized]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderProfileLocalized](
	[Language] [tinyint] NOT NULL,
	[ProviderProfile] [int] NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[SecondName] [varchar](200) NOT NULL,
	[CompanyName] [varchar](200) NOT NULL,
	[OwnerName] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ProviderProfileLocalized] PRIMARY KEY CLUSTERED 
(
	[Language] ASC,
	[ProviderProfile] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](10) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vehicle]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vehicle](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [varchar](200) NOT NULL,
	[Model] [varchar](200) NOT NULL,
	[Type] [varchar](200) NOT NULL,
	[Year] [varchar](200) NOT NULL,
	[Delivary] [int] NOT NULL,
 CONSTRAINT [PK_Vehicle] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VerificationToken]    Script Date: 11/10/2024 2:21:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VerificationToken](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MembershipUser] [int] NOT NULL,
	[Value] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_VerificationToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MembershipUser] ADD  CONSTRAINT [DF__AspNetUser__Role__5D60DB10]  DEFAULT (CONVERT([smallint],(0))) FOR [Role]
GO
ALTER TABLE [dbo].[Card]  WITH CHECK ADD  CONSTRAINT [FK_Card_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[Customer] ([Party])
GO
ALTER TABLE [dbo].[Card] CHECK CONSTRAINT [FK_Card_Customer]
GO
ALTER TABLE [dbo].[CountryLocalized]  WITH CHECK ADD  CONSTRAINT [FK_CountryLocalized_Country] FOREIGN KEY([Country])
REFERENCES [dbo].[Country] ([Id])
GO
ALTER TABLE [dbo].[CountryLocalized] CHECK CONSTRAINT [FK_CountryLocalized_Country]
GO
ALTER TABLE [dbo].[CountryLocalized]  WITH CHECK ADD  CONSTRAINT [FK_CountryLocalized_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[CountryLocalized] CHECK CONSTRAINT [FK_CountryLocalized_Language]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Party1] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_Party1]
GO
ALTER TABLE [dbo].[CustomerProfile]  WITH CHECK ADD  CONSTRAINT [FK_CustomerProfile_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[Customer] ([Party])
GO
ALTER TABLE [dbo].[CustomerProfile] CHECK CONSTRAINT [FK_CustomerProfile_Customer]
GO
ALTER TABLE [dbo].[CustomerProfileLocalized]  WITH CHECK ADD  CONSTRAINT [FK_CustomerProfileLocalized_CustomerProfile] FOREIGN KEY([CustomerProfile])
REFERENCES [dbo].[CustomerProfile] ([Id])
GO
ALTER TABLE [dbo].[CustomerProfileLocalized] CHECK CONSTRAINT [FK_CustomerProfileLocalized_CustomerProfile]
GO
ALTER TABLE [dbo].[CustomerProfileLocalized]  WITH CHECK ADD  CONSTRAINT [FK_CustomerProfileLocalized_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[CustomerProfileLocalized] CHECK CONSTRAINT [FK_CustomerProfileLocalized_Language]
GO
ALTER TABLE [dbo].[DelivaryDocument]  WITH CHECK ADD  CONSTRAINT [FK_Document_Delivary] FOREIGN KEY([Delivary])
REFERENCES [dbo].[Delivery] ([Party])
GO
ALTER TABLE [dbo].[DelivaryDocument] CHECK CONSTRAINT [FK_Document_Delivary]
GO
ALTER TABLE [dbo].[DelivaryDocument]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentType] FOREIGN KEY([DocumentType])
REFERENCES [dbo].[DocumentType] ([Id])
GO
ALTER TABLE [dbo].[DelivaryDocument] CHECK CONSTRAINT [FK_Document_DocumentType]
GO
ALTER TABLE [dbo].[DelivaryProfile]  WITH CHECK ADD  CONSTRAINT [FK_DelivaryProfile_Delivary] FOREIGN KEY([Delivary])
REFERENCES [dbo].[Delivery] ([Party])
GO
ALTER TABLE [dbo].[DelivaryProfile] CHECK CONSTRAINT [FK_DelivaryProfile_Delivary]
GO
ALTER TABLE [dbo].[DelivaryProfileLocalized]  WITH CHECK ADD  CONSTRAINT [FK_DelivaryProfileLocalized_DelivaryProfile] FOREIGN KEY([DelivaryProfile])
REFERENCES [dbo].[DelivaryProfile] ([Id])
GO
ALTER TABLE [dbo].[DelivaryProfileLocalized] CHECK CONSTRAINT [FK_DelivaryProfileLocalized_DelivaryProfile]
GO
ALTER TABLE [dbo].[DelivaryProfileLocalized]  WITH CHECK ADD  CONSTRAINT [FK_DelivaryProfileLocalized_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[DelivaryProfileLocalized] CHECK CONSTRAINT [FK_DelivaryProfileLocalized_Language]
GO
ALTER TABLE [dbo].[Delivery]  WITH CHECK ADD  CONSTRAINT [FK_Delivary_Party] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[Delivery] CHECK CONSTRAINT [FK_Delivary_Party]
GO
ALTER TABLE [dbo].[DiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_DiscountCode_DiscountType] FOREIGN KEY([DiscountType])
REFERENCES [dbo].[DiscountType] ([Id])
GO
ALTER TABLE [dbo].[DiscountCode] CHECK CONSTRAINT [FK_DiscountCode_DiscountType]
GO
ALTER TABLE [dbo].[Location]  WITH CHECK ADD  CONSTRAINT [FK_Location_Party] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[Location] CHECK CONSTRAINT [FK_Location_Party]
GO
ALTER TABLE [dbo].[LocationDetails]  WITH CHECK ADD  CONSTRAINT [FK_LocationDetails_Location] FOREIGN KEY([Location])
REFERENCES [dbo].[Location] ([Id])
GO
ALTER TABLE [dbo].[LocationDetails] CHECK CONSTRAINT [FK_LocationDetails_Location]
GO
ALTER TABLE [dbo].[MembershipUser]  WITH CHECK ADD  CONSTRAINT [FK_MembershipUser_Party] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[MembershipUser] CHECK CONSTRAINT [FK_MembershipUser_Party]
GO
ALTER TABLE [dbo].[MembershipUser]  WITH CHECK ADD  CONSTRAINT [FK_MembershipUser_Role] FOREIGN KEY([Role])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[MembershipUser] CHECK CONSTRAINT [FK_MembershipUser_Role]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[Customer] ([Party])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Customer]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_OrderStatus] FOREIGN KEY([OrderStatus])
REFERENCES [dbo].[OrderStatus] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_OrderStatus]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Provider]
GO
ALTER TABLE [dbo].[OrderActivity]  WITH CHECK ADD  CONSTRAINT [FK_OrderActivity_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderActivity] CHECK CONSTRAINT [FK_OrderActivity_Order]
GO
ALTER TABLE [dbo].[OrderActivity]  WITH CHECK ADD  CONSTRAINT [FK_OrderActivity_OrderActivityType] FOREIGN KEY([OrderActivityType])
REFERENCES [dbo].[OrderActivityType] ([Id])
GO
ALTER TABLE [dbo].[OrderActivity] CHECK CONSTRAINT [FK_OrderActivity_OrderActivityType]
GO
ALTER TABLE [dbo].[OrderActivity]  WITH CHECK ADD  CONSTRAINT [FK_OrderActivity_Party] FOREIGN KEY([OperatedBy])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[OrderActivity] CHECK CONSTRAINT [FK_OrderActivity_Party]
GO
ALTER TABLE [dbo].[OrderDiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_OrderDiscountCode_DiscountCode] FOREIGN KEY([DiscountCode])
REFERENCES [dbo].[DiscountCode] ([Id])
GO
ALTER TABLE [dbo].[OrderDiscountCode] CHECK CONSTRAINT [FK_OrderDiscountCode_DiscountCode]
GO
ALTER TABLE [dbo].[OrderDiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_OrderDiscountCode_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderDiscountCode] CHECK CONSTRAINT [FK_OrderDiscountCode_Order]
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
ALTER TABLE [dbo].[OrderItemDiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemDiscountCode_DiscountCode] FOREIGN KEY([DiscountCode])
REFERENCES [dbo].[DiscountCode] ([Id])
GO
ALTER TABLE [dbo].[OrderItemDiscountCode] CHECK CONSTRAINT [FK_OrderItemDiscountCode_DiscountCode]
GO
ALTER TABLE [dbo].[OrderItemDiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemDiscountCode_OrderItem] FOREIGN KEY([OrderItem])
REFERENCES [dbo].[OrderItem] ([Id])
GO
ALTER TABLE [dbo].[OrderItemDiscountCode] CHECK CONSTRAINT [FK_OrderItemDiscountCode_OrderItem]
GO
ALTER TABLE [dbo].[PartyLocalized]  WITH CHECK ADD  CONSTRAINT [FK_PartyLocalized_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[PartyLocalized] CHECK CONSTRAINT [FK_PartyLocalized_Language]
GO
ALTER TABLE [dbo].[PartyLocalized]  WITH CHECK ADD  CONSTRAINT [FK_PartyLocalized_Party] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[PartyLocalized] CHECK CONSTRAINT [FK_PartyLocalized_Party]
GO
ALTER TABLE [dbo].[PhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_PhoneNumber_Party] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[PhoneNumber] CHECK CONSTRAINT [FK_PhoneNumber_Party]
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
ALTER TABLE [dbo].[ProductDiscount]  WITH CHECK ADD  CONSTRAINT [FK_ProductDiscount_DiscountCode] FOREIGN KEY([DiscountCode])
REFERENCES [dbo].[DiscountCode] ([Id])
GO
ALTER TABLE [dbo].[ProductDiscount] CHECK CONSTRAINT [FK_ProductDiscount_DiscountCode]
GO
ALTER TABLE [dbo].[ProductDiscount]  WITH CHECK ADD  CONSTRAINT [FK_ProductDiscount_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductDiscount] CHECK CONSTRAINT [FK_ProductDiscount_Product]
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
ALTER TABLE [dbo].[ProductOfferDiscount]  WITH CHECK ADD  CONSTRAINT [FK_ProductOfferDiscount_DiscountCode] FOREIGN KEY([DiscountCode])
REFERENCES [dbo].[DiscountCode] ([Id])
GO
ALTER TABLE [dbo].[ProductOfferDiscount] CHECK CONSTRAINT [FK_ProductOfferDiscount_DiscountCode]
GO
ALTER TABLE [dbo].[ProductOfferDiscount]  WITH CHECK ADD  CONSTRAINT [FK_ProductOfferDiscount_ProductOffer] FOREIGN KEY([ProductOffer])
REFERENCES [dbo].[ProductOffer] ([Id])
GO
ALTER TABLE [dbo].[ProductOfferDiscount] CHECK CONSTRAINT [FK_ProductOfferDiscount_ProductOffer]
GO
ALTER TABLE [dbo].[ProductRequest]  WITH CHECK ADD  CONSTRAINT [FK_ProductRequest_ProductRequestStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[ProductRequestStatus] ([Id])
GO
ALTER TABLE [dbo].[ProductRequest] CHECK CONSTRAINT [FK_ProductRequest_ProductRequestStatus]
GO
ALTER TABLE [dbo].[ProductRequestDataLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProductRequestDataLocalized_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[ProductRequestDataLocalized] CHECK CONSTRAINT [FK_ProductRequestDataLocalized_Language]
GO
ALTER TABLE [dbo].[ProductRequestDataLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProductRequestDataLocalized_ProductRequest] FOREIGN KEY([ProductRequest])
REFERENCES [dbo].[ProductRequest] ([Id])
GO
ALTER TABLE [dbo].[ProductRequestDataLocalized] CHECK CONSTRAINT [FK_ProductRequestDataLocalized_ProductRequest]
GO
ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_Party] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_Party]
GO
ALTER TABLE [dbo].[ProviderDocument]  WITH CHECK ADD  CONSTRAINT [FK_ProviderDocument_DocumentType] FOREIGN KEY([DocumentType])
REFERENCES [dbo].[DocumentType] ([Id])
GO
ALTER TABLE [dbo].[ProviderDocument] CHECK CONSTRAINT [FK_ProviderDocument_DocumentType]
GO
ALTER TABLE [dbo].[ProviderDocument]  WITH CHECK ADD  CONSTRAINT [FK_ProviderDocument_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[ProviderDocument] CHECK CONSTRAINT [FK_ProviderDocument_Provider]
GO
ALTER TABLE [dbo].[ProviderProfile]  WITH CHECK ADD  CONSTRAINT [FK_ProviderProfile_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[ProviderProfile] CHECK CONSTRAINT [FK_ProviderProfile_Provider]
GO
ALTER TABLE [dbo].[ProviderProfileLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProviderProfileLocalized_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[ProviderProfileLocalized] CHECK CONSTRAINT [FK_ProviderProfileLocalized_Language]
GO
ALTER TABLE [dbo].[ProviderProfileLocalized]  WITH CHECK ADD  CONSTRAINT [FK_ProviderProfileLocalized_ProviderProfile] FOREIGN KEY([ProviderProfile])
REFERENCES [dbo].[ProviderProfile] ([Id])
GO
ALTER TABLE [dbo].[ProviderProfileLocalized] CHECK CONSTRAINT [FK_ProviderProfileLocalized_ProviderProfile]
GO
ALTER TABLE [dbo].[Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_Vehicle_Delivary] FOREIGN KEY([Delivary])
REFERENCES [dbo].[Delivery] ([Party])
GO
ALTER TABLE [dbo].[Vehicle] CHECK CONSTRAINT [FK_Vehicle_Delivary]
GO
ALTER TABLE [dbo].[VerificationToken]  WITH CHECK ADD  CONSTRAINT [FK_VerificationToken_MembershipUser] FOREIGN KEY([MembershipUser])
REFERENCES [dbo].[MembershipUser] ([Id])
GO
ALTER TABLE [dbo].[VerificationToken] CHECK CONSTRAINT [FK_VerificationToken_MembershipUser]
GO
