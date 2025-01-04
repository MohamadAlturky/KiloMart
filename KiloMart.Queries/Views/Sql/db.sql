USE [db_aaf400_kilomartmasterdb]
GO
/****** Object:  Table [dbo].[AppSettings]    Script Date: 1/4/2025 7:55:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppSettings](
	[Key] [int] NOT NULL,
	[Value] [varchar](50) NOT NULL,
 CONSTRAINT [PK_AppSettings] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Card]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[Cart]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cart](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Product] [int] NOT NULL,
	[Quantity] [float] NOT NULL,
	[Customer] [int] NOT NULL,
 CONSTRAINT [PK_Cart] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Configs]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configs](
	[Key] [varchar](100) NOT NULL,
	[Value] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Configs] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[CustomerProfile]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[Deal]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Deal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Product] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[OffPercentage] [float] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Deal] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DelivaryDocument]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[DelivaryProfile]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[Delivery]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[DeliveryActivity]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryActivity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Value] [float] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Delivery] [int] NOT NULL,
 CONSTRAINT [PK_DeliveryActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryActivityType]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryActivityType](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DeliveryActivityType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryProfileHistory]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryProfileHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[SecondName] [varchar](100) NOT NULL,
	[NationalName] [varchar](100) NOT NULL,
	[NationalId] [varchar](100) NOT NULL,
	[LicenseNumber] [varchar](100) NOT NULL,
	[LicenseExpiredDate] [datetime] NOT NULL,
	[DrivingLicenseNumber] [varchar](100) NOT NULL,
	[DrivingLicenseExpiredDate] [datetime] NOT NULL,
	[VehicleNumber] [varchar](100) NOT NULL,
	[VehicleModel] [varchar](100) NOT NULL,
	[VehicleType] [varchar](100) NOT NULL,
	[VehicleYear] [varchar](10) NOT NULL,
	[VehiclePhotoFileUrl] [varchar](300) NOT NULL,
	[DrivingLicenseFileUrl] [varchar](300) NOT NULL,
	[VehicleLicenseFileUrl] [varchar](300) NOT NULL,
	[NationalIqamaIDFileUrl] [varchar](300) NOT NULL,
	[SubmitDate] [datetime] NOT NULL,
	[ReviewDate] [datetime] NULL,
	[DeliveryId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsRejected] [bit] NOT NULL,
	[IsAccepted] [bit] NOT NULL,
	[ReviewDescription] [varchar](300) NULL,
 CONSTRAINT [PK__Delivery__3214EC07B1DA8363] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryWallet]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryWallet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [float] NOT NULL,
	[Delivery] [int] NOT NULL,
 CONSTRAINT [PK_DeliveryWallet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiscountCode]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[DiscountType]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscountType](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DiscountType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[DriverFreeFee]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DriverFreeFee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DriverFreeFee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FAQ]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FAQ](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Question] [varchar](200) NOT NULL,
	[Answer] [varchar](300) NOT NULL,
	[Language] [tinyint] NOT NULL,
	[Type] [tinyint] NOT NULL,
 CONSTRAINT [PK_FAQ] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FavoriteProducts]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FavoriteProducts](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Customer] [int] NOT NULL,
	[Product] [int] NOT NULL,
 CONSTRAINT [PK_FavoriteProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Language]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[Location]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[LocationDetails]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[MembershipUser]    Script Date: 1/4/2025 7:55:22 AM ******/
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
	[Language] [tinyint] NOT NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[Content] [varchar](2000) NOT NULL,
	[Date] [datetime] NOT NULL,
	[ForParty] [int] NOT NULL,
	[JsonPayLoad] [varchar](2000) NOT NULL,
	[IsRead] [bit] NOT NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderStatus] [tinyint] NOT NULL,
	[TotalPrice] [money] NOT NULL,
	[TransactionId] [varchar](50) NOT NULL,
	[Date] [datetime] NOT NULL,
	[PaymentType] [tinyint] NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[DeliveryFee] [money] NOT NULL,
	[SystemFee] [money] NOT NULL,
	[ItemsPrice] [money] NOT NULL,
	[SpecialRequest] [varchar](300) NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderActivity]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[OrderActivityType]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderActivityType](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_OrderActivityType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderCustomerInformation]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderCustomerInformation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [bigint] NOT NULL,
	[Customer] [int] NOT NULL,
	[Location] [int] NOT NULL,
 CONSTRAINT [PK_OrderCustomerInformation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDeliveryInformation]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDeliveryInformation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [bigint] NOT NULL,
	[Delivery] [int] NOT NULL,
 CONSTRAINT [PK_OrderDeliveryInformation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDiscountCode]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[OrderItemDiscountCode]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[OrderProduct]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Order] [bigint] NOT NULL,
	[Product] [int] NOT NULL,
	[Quantity] [float] NOT NULL,
 CONSTRAINT [PK_OrderProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderProductOffer]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderProductOffer](
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
/****** Object:  Table [dbo].[OrderProviderInformation]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderProviderInformation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [bigint] NOT NULL,
	[Provider] [int] NOT NULL,
	[Location] [int] NOT NULL,
 CONSTRAINT [PK_OrderProviderInformation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatus]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatus](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Party]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[PaymentType]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentType](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhoneNumber]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[Product]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductCategoryLocalized]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductDiscount]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductLocalized]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductOffer]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductOfferDiscount]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductRequest]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductRequestDataLocalized]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProductRequestStatus]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductRequestStatus](
	[Id] [tinyint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ProductRequestStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Provider]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProviderActivity]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderActivity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Value] [float] NOT NULL,
	[Provider] [int] NOT NULL,
	[Order] [bigint] NOT NULL,
 CONSTRAINT [PK_ProviderActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProviderDocument]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProviderProfile]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[ProviderProfileHistory]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderProfileHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[SecondName] [varchar](100) NOT NULL,
	[NationalApprovalId] [varchar](100) NOT NULL,
	[CompanyName] [varchar](200) NOT NULL,
	[OwnerName] [varchar](200) NOT NULL,
	[OwnerNationalId] [varchar](100) NOT NULL,
	[OwnershipDocumentFileUrl] [varchar](300) NOT NULL,
	[OwnerNationalApprovalFileUrl] [varchar](300) NOT NULL,
	[LocationName] [varchar](200) NOT NULL,
	[Longitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[BuildingType] [varchar](100) NOT NULL,
	[BuildingNumber] [varchar](100) NOT NULL,
	[FloorNumber] [varchar](100) NOT NULL,
	[ApartmentNumber] [varchar](100) NOT NULL,
	[StreetNumber] [varchar](100) NOT NULL,
	[PhoneNumber] [varchar](100) NOT NULL,
	[IsAccepted] [bit] NOT NULL,
	[IsRejected] [bit] NOT NULL,
	[SubmitDate] [datetime] NOT NULL,
	[ReviewDate] [datetime] NULL,
	[ProviderId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ReviewDescription] [varchar](300) NULL,
 CONSTRAINT [PK__Provider__3214EC076F2F8415] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProviderWallet]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderWallet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [float] NOT NULL,
	[Provider] [int] NOT NULL,
 CONSTRAINT [PK_ProviderWallet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResetPasswordCode]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResetPasswordCode](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[MembershipUser] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [PK_ResetPasswordCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[SearchHistory]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Customer] [int] NOT NULL,
	[Term] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SearchHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sessions]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sessions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Token] [varchar](max) NOT NULL,
	[UserId] [int] NOT NULL,
	[ExpireDate] [datetime] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Code] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SliderItem]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SliderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageUrl] [varchar](100) NOT NULL,
	[Target] [int] NULL,
 CONSTRAINT [PK_SliderItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemActivity]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemActivity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Value] [float] NOT NULL,
	[Order] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemSettings]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemSettings](
	[Id] [int] NOT NULL,
	[DeliveryOrderFee] [float] NOT NULL,
	[SystemOrderFee] [float] NOT NULL,
	[CancelOrderWhenNoProviderHasAllProducts] [bit] NOT NULL,
	[TimeInMinutesToMakeTheCircleBigger] [int] NOT NULL,
	[CircleRaduis] [float] NOT NULL,
	[MaxMinutesToCancelOrderWaitingAProvider] [int] NOT NULL,
	[MinOrderValue] [float] NOT NULL,
	[DistanceToAdd] [float] NOT NULL,
	[MaxDistanceToAdd] [float] NOT NULL,
	[RaduisForGetProducts] [float] NOT NULL,
 CONSTRAINT [PK_SystemSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vehicle]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[VerificationToken]    Script Date: 1/4/2025 7:55:22 AM ******/
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
/****** Object:  Table [dbo].[Withdraw]    Script Date: 1/4/2025 7:55:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Withdraw](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Delivery] [int] NOT NULL,
	[BankAccountNumber] [varchar](50) NOT NULL,
	[IBanNumber] [varchar](50) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Done] [bit] NOT NULL,
 CONSTRAINT [PK_Withdraw] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__First__15702A09]  DEFAULT ('') FOR [FirstName]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__Secon__16644E42]  DEFAULT ('') FOR [SecondName]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__Natio__1758727B]  DEFAULT ('') FOR [NationalName]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__Natio__184C96B4]  DEFAULT ('') FOR [NationalId]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__Licen__1940BAED]  DEFAULT ('') FOR [LicenseNumber]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__Drivi__1A34DF26]  DEFAULT ('') FOR [DrivingLicenseNumber]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__Creat__1D114BD1]  DEFAULT (getdate()) FOR [SubmitDate]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__IsRej__1C1D2798]  DEFAULT ((0)) FOR [IsRejected]
GO
ALTER TABLE [dbo].[DeliveryProfileHistory] ADD  CONSTRAINT [DF__DeliveryP__IsAcc__1B29035F]  DEFAULT ((0)) FOR [IsAccepted]
GO
ALTER TABLE [dbo].[MembershipUser] ADD  CONSTRAINT [DF__AspNetUser__Role__5D60DB10]  DEFAULT (CONVERT([smallint],(0))) FOR [Role]
GO
ALTER TABLE [dbo].[MembershipUser] ADD  CONSTRAINT [DF__Membershi__Langu__3CF40B7E]  DEFAULT ((1)) FOR [Language]
GO
ALTER TABLE [dbo].[ProviderProfileHistory] ADD  CONSTRAINT [DF__ProviderP__IsAcc__21D600EE]  DEFAULT ((0)) FOR [IsAccepted]
GO
ALTER TABLE [dbo].[ProviderProfileHistory] ADD  CONSTRAINT [DF__ProviderP__IsRej__22CA2527]  DEFAULT ((0)) FOR [IsRejected]
GO
ALTER TABLE [dbo].[ProviderProfileHistory] ADD  CONSTRAINT [DF__ProviderP__Creat__23BE4960]  DEFAULT (getdate()) FOR [SubmitDate]
GO
ALTER TABLE [dbo].[Card]  WITH CHECK ADD  CONSTRAINT [FK_Card_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[Customer] ([Party])
GO
ALTER TABLE [dbo].[Card] CHECK CONSTRAINT [FK_Card_Customer]
GO
ALTER TABLE [dbo].[Cart]  WITH CHECK ADD  CONSTRAINT [FK_Cart_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[Customer] ([Party])
GO
ALTER TABLE [dbo].[Cart] CHECK CONSTRAINT [FK_Cart_Customer]
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
ALTER TABLE [dbo].[Deal]  WITH CHECK ADD  CONSTRAINT [FK_Deal_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[Deal] CHECK CONSTRAINT [FK_Deal_Product]
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
ALTER TABLE [dbo].[Delivery]  WITH CHECK ADD  CONSTRAINT [FK_Delivary_Party] FOREIGN KEY([Party])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[Delivery] CHECK CONSTRAINT [FK_Delivary_Party]
GO
ALTER TABLE [dbo].[DeliveryActivity]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryActivity_Delivery] FOREIGN KEY([Delivery])
REFERENCES [dbo].[Delivery] ([Party])
GO
ALTER TABLE [dbo].[DeliveryActivity] CHECK CONSTRAINT [FK_DeliveryActivity_Delivery]
GO
ALTER TABLE [dbo].[DeliveryActivity]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryActivity_DeliveryActivityType] FOREIGN KEY([Type])
REFERENCES [dbo].[DeliveryActivityType] ([Id])
GO
ALTER TABLE [dbo].[DeliveryActivity] CHECK CONSTRAINT [FK_DeliveryActivity_DeliveryActivityType]
GO
ALTER TABLE [dbo].[DeliveryWallet]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryWallet_Delivery] FOREIGN KEY([Delivery])
REFERENCES [dbo].[Delivery] ([Party])
GO
ALTER TABLE [dbo].[DeliveryWallet] CHECK CONSTRAINT [FK_DeliveryWallet_Delivery]
GO
ALTER TABLE [dbo].[DiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_DiscountCode_DiscountType] FOREIGN KEY([DiscountType])
REFERENCES [dbo].[DiscountType] ([Id])
GO
ALTER TABLE [dbo].[DiscountCode] CHECK CONSTRAINT [FK_DiscountCode_DiscountType]
GO
ALTER TABLE [dbo].[FAQ]  WITH CHECK ADD  CONSTRAINT [FK_FAQ_Language] FOREIGN KEY([Language])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[FAQ] CHECK CONSTRAINT [FK_FAQ_Language]
GO
ALTER TABLE [dbo].[FavoriteProducts]  WITH CHECK ADD  CONSTRAINT [FK_FavoriteProducts_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[Customer] ([Party])
GO
ALTER TABLE [dbo].[FavoriteProducts] CHECK CONSTRAINT [FK_FavoriteProducts_Customer]
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
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Party] FOREIGN KEY([ForParty])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Party]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_OrderStatus] FOREIGN KEY([OrderStatus])
REFERENCES [dbo].[OrderStatus] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_OrderStatus]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_PaymentType] FOREIGN KEY([PaymentType])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_PaymentType]
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
ALTER TABLE [dbo].[OrderCustomerInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderCustomerInformation_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[Customer] ([Party])
GO
ALTER TABLE [dbo].[OrderCustomerInformation] CHECK CONSTRAINT [FK_OrderCustomerInformation_Customer]
GO
ALTER TABLE [dbo].[OrderCustomerInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderCustomerInformation_Location] FOREIGN KEY([Location])
REFERENCES [dbo].[Location] ([Id])
GO
ALTER TABLE [dbo].[OrderCustomerInformation] CHECK CONSTRAINT [FK_OrderCustomerInformation_Location]
GO
ALTER TABLE [dbo].[OrderCustomerInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderCustomerInformation_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderCustomerInformation] CHECK CONSTRAINT [FK_OrderCustomerInformation_Order]
GO
ALTER TABLE [dbo].[OrderDeliveryInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderDeliveryInformation_Delivery] FOREIGN KEY([Delivery])
REFERENCES [dbo].[Delivery] ([Party])
GO
ALTER TABLE [dbo].[OrderDeliveryInformation] CHECK CONSTRAINT [FK_OrderDeliveryInformation_Delivery]
GO
ALTER TABLE [dbo].[OrderDeliveryInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderDeliveryInformation_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderDeliveryInformation] CHECK CONSTRAINT [FK_OrderDeliveryInformation_Order]
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
ALTER TABLE [dbo].[OrderItemDiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemDiscountCode_DiscountCode] FOREIGN KEY([DiscountCode])
REFERENCES [dbo].[DiscountCode] ([Id])
GO
ALTER TABLE [dbo].[OrderItemDiscountCode] CHECK CONSTRAINT [FK_OrderItemDiscountCode_DiscountCode]
GO
ALTER TABLE [dbo].[OrderItemDiscountCode]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemDiscountCode_OrderItem] FOREIGN KEY([OrderItem])
REFERENCES [dbo].[OrderProductOffer] ([Id])
GO
ALTER TABLE [dbo].[OrderItemDiscountCode] CHECK CONSTRAINT [FK_OrderItemDiscountCode_OrderItem]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrderProduct_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrderProduct_Order]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrderProduct_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrderProduct_Product]
GO
ALTER TABLE [dbo].[OrderProductOffer]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderProductOffer] CHECK CONSTRAINT [FK_OrderItem_Order]
GO
ALTER TABLE [dbo].[OrderProductOffer]  WITH CHECK ADD  CONSTRAINT [FK_OrderProductOffer_ProductOffer] FOREIGN KEY([ProductOffer])
REFERENCES [dbo].[ProductOffer] ([Id])
GO
ALTER TABLE [dbo].[OrderProductOffer] CHECK CONSTRAINT [FK_OrderProductOffer_ProductOffer]
GO
ALTER TABLE [dbo].[OrderProviderInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderProviderInformation_Location] FOREIGN KEY([Location])
REFERENCES [dbo].[Location] ([Id])
GO
ALTER TABLE [dbo].[OrderProviderInformation] CHECK CONSTRAINT [FK_OrderProviderInformation_Location]
GO
ALTER TABLE [dbo].[OrderProviderInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderProviderInformation_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderProviderInformation] CHECK CONSTRAINT [FK_OrderProviderInformation_Order]
GO
ALTER TABLE [dbo].[OrderProviderInformation]  WITH CHECK ADD  CONSTRAINT [FK_OrderProviderInformation_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[OrderProviderInformation] CHECK CONSTRAINT [FK_OrderProviderInformation_Provider]
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
ALTER TABLE [dbo].[ProductOffer]  WITH CHECK ADD  CONSTRAINT [FK_ProductOffer_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[ProductOffer] CHECK CONSTRAINT [FK_ProductOffer_Provider]
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
ALTER TABLE [dbo].[ProductRequest]  WITH CHECK ADD  CONSTRAINT [FK_ProductRequest_ProductCategory] FOREIGN KEY([ProductCategory])
REFERENCES [dbo].[ProductCategory] ([Id])
GO
ALTER TABLE [dbo].[ProductRequest] CHECK CONSTRAINT [FK_ProductRequest_ProductCategory]
GO
ALTER TABLE [dbo].[ProductRequest]  WITH CHECK ADD  CONSTRAINT [FK_ProductRequest_ProductRequestStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[ProductRequestStatus] ([Id])
GO
ALTER TABLE [dbo].[ProductRequest] CHECK CONSTRAINT [FK_ProductRequest_ProductRequestStatus]
GO
ALTER TABLE [dbo].[ProductRequest]  WITH CHECK ADD  CONSTRAINT [FK_ProductRequest_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[ProductRequest] CHECK CONSTRAINT [FK_ProductRequest_Provider]
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
ALTER TABLE [dbo].[ProviderActivity]  WITH CHECK ADD  CONSTRAINT [FK_ProviderActivity_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[ProviderActivity] CHECK CONSTRAINT [FK_ProviderActivity_Order]
GO
ALTER TABLE [dbo].[ProviderActivity]  WITH CHECK ADD  CONSTRAINT [FK_ProviderActivity_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[ProviderActivity] CHECK CONSTRAINT [FK_ProviderActivity_Provider]
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
ALTER TABLE [dbo].[ProviderWallet]  WITH CHECK ADD  CONSTRAINT [FK_ProviderWallet_Provider] FOREIGN KEY([Provider])
REFERENCES [dbo].[Provider] ([Party])
GO
ALTER TABLE [dbo].[ProviderWallet] CHECK CONSTRAINT [FK_ProviderWallet_Provider]
GO
ALTER TABLE [dbo].[ResetPasswordCode]  WITH CHECK ADD  CONSTRAINT [FK_ResetPasswordCode_MembershipUser] FOREIGN KEY([MembershipUser])
REFERENCES [dbo].[MembershipUser] ([Id])
GO
ALTER TABLE [dbo].[ResetPasswordCode] CHECK CONSTRAINT [FK_ResetPasswordCode_MembershipUser]
GO
ALTER TABLE [dbo].[SearchHistory]  WITH CHECK ADD  CONSTRAINT [FK_SearchHistory_Party] FOREIGN KEY([Customer])
REFERENCES [dbo].[Party] ([Id])
GO
ALTER TABLE [dbo].[SearchHistory] CHECK CONSTRAINT [FK_SearchHistory_Party]
GO
ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_MembershipUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[MembershipUser] ([Id])
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_MembershipUser]
GO
ALTER TABLE [dbo].[SliderItem]  WITH CHECK ADD  CONSTRAINT [FK_SliderItem_SliderItem] FOREIGN KEY([Target])
REFERENCES [dbo].[SliderItem] ([Id])
GO
ALTER TABLE [dbo].[SliderItem] CHECK CONSTRAINT [FK_SliderItem_SliderItem]
GO
ALTER TABLE [dbo].[SystemActivity]  WITH CHECK ADD  CONSTRAINT [FK_SystemActivity_Order] FOREIGN KEY([Order])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[SystemActivity] CHECK CONSTRAINT [FK_SystemActivity_Order]
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
ALTER TABLE [dbo].[Withdraw]  WITH CHECK ADD  CONSTRAINT [FK_Withdraw_Delivery] FOREIGN KEY([Delivery])
REFERENCES [dbo].[Delivery] ([Party])
GO
ALTER TABLE [dbo].[Withdraw] CHECK CONSTRAINT [FK_Withdraw_Delivery]
GO
