using System;
using System.Collections.Generic;
using KiloMart.DataAccess.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace KiloMart.DataAccess.EFCore.Data;

public partial class KilomartDbContext : DbContext
{
    public KilomartDbContext()
    {
    }

    public KilomartDbContext(DbContextOptions<KilomartDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppSetting> AppSettings { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Config> Configs { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerProfile> CustomerProfiles { get; set; }

    public virtual DbSet<Deal> Deals { get; set; }

    public virtual DbSet<DelivaryDocument> DelivaryDocuments { get; set; }

    public virtual DbSet<DelivaryProfile> DelivaryProfiles { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<DeliveryActivity> DeliveryActivities { get; set; }

    public virtual DbSet<DeliveryActivityType> DeliveryActivityTypes { get; set; }

    public virtual DbSet<DeliveryProfileHistory> DeliveryProfileHistories { get; set; }

    public virtual DbSet<DeliveryWallet> DeliveryWallets { get; set; }

    public virtual DbSet<DiscountCode> DiscountCodes { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<DriverFreeFee> DriverFreeFees { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<FavoriteProduct> FavoriteProducts { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<LocationDetail> LocationDetails { get; set; }

    public virtual DbSet<MembershipUser> MembershipUsers { get; set; }

    public virtual DbSet<MobileAppConfiguration> MobileAppConfigurations { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderActivity> OrderActivities { get; set; }

    public virtual DbSet<OrderActivityType> OrderActivityTypes { get; set; }

    public virtual DbSet<OrderCustomerInformation> OrderCustomerInformations { get; set; }

    public virtual DbSet<OrderDeliveryInformation> OrderDeliveryInformations { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderProductOffer> OrderProductOffers { get; set; }

    public virtual DbSet<OrderProviderInformation> OrderProviderInformations { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Party> Parties { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductCategoryLocalized> ProductCategoryLocalizeds { get; set; }

    public virtual DbSet<ProductDiscount> ProductDiscounts { get; set; }

    public virtual DbSet<ProductLocalized> ProductLocalizeds { get; set; }

    public virtual DbSet<ProductOffer> ProductOffers { get; set; }

    public virtual DbSet<ProductOfferDiscount> ProductOfferDiscounts { get; set; }

    public virtual DbSet<ProductRequest> ProductRequests { get; set; }

    public virtual DbSet<ProductRequestDataLocalized> ProductRequestDataLocalizeds { get; set; }

    public virtual DbSet<ProductRequestStatus> ProductRequestStatuses { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<ProviderActivity> ProviderActivities { get; set; }

    public virtual DbSet<ProviderDocument> ProviderDocuments { get; set; }

    public virtual DbSet<ProviderProfile> ProviderProfiles { get; set; }

    public virtual DbSet<ProviderProfileHistory> ProviderProfileHistories { get; set; }

    public virtual DbSet<ProviderWallet> ProviderWallets { get; set; }

    public virtual DbSet<ResetPasswordCode> ResetPasswordCodes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SearchHistory> SearchHistories { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SliderItem> SliderItems { get; set; }

    public virtual DbSet<SystemActivity> SystemActivities { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<TextualConfig> TextualConfigs { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VerificationToken> VerificationTokens { get; set; }

    public virtual DbSet<Withdraw> Withdraws { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=SQL9001.site4now.net\\\\\\\\SQLEXPRESS01;Data Source=SQL9001.site4now.net;Initial Catalog=db_aaf400_kilomartmasterdb;User Id=db_aaf400_kilomartmasterdb_admin;Password=A@123456789;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Trusted_Connection=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Arabic_CI_AS");

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(e => e.Key);

            entity.Property(e => e.Key).ValueGeneratedNever();
            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("Card");

            entity.HasIndex(e => e.IsActive, "IX_Card");

            entity.HasIndex(e => e.Customer, "IX_Card_1");

            entity.Property(e => e.HolderName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Number)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SecurityCode)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Card_Customer");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("Cart");

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Customer");
        });

        modelBuilder.Entity<Config>(entity =>
        {
            entity.HasKey(e => e.Key);

            entity.Property(e => e.Key)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Party).HasName("PK_Customer_1");

            entity.ToTable("Customer");

            entity.Property(e => e.Party).ValueGeneratedNever();

            entity.HasOne(d => d.PartyNavigation).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.Party)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Party1");
        });

        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.ToTable("CustomerProfile");

            entity.HasIndex(e => e.Customer, "IX_CustomerProfile").IsUnique();

            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NationalId)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NationalName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SecondName)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.CustomerNavigation).WithOne(p => p.CustomerProfile)
                .HasForeignKey<CustomerProfile>(d => d.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerProfile_Customer");
        });

        modelBuilder.Entity<Deal>(entity =>
        {
            entity.ToTable("Deal");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.Deals)
                .HasForeignKey(d => d.Product)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deal_Product");
        });

        modelBuilder.Entity<DelivaryDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Document");

            entity.ToTable("DelivaryDocument");

            entity.HasIndex(e => e.DocumentType, "IX_DelivaryDocument");

            entity.HasIndex(e => e.Delivary, "IX_DelivaryDocument_1");

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.DelivaryNavigation).WithMany(p => p.DelivaryDocuments)
                .HasForeignKey(d => d.Delivary)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Document_Delivary");

            entity.HasOne(d => d.DocumentTypeNavigation).WithMany(p => p.DelivaryDocuments)
                .HasForeignKey(d => d.DocumentType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Document_DocumentType");
        });

        modelBuilder.Entity<DelivaryProfile>(entity =>
        {
            entity.ToTable("DelivaryProfile");

            entity.HasIndex(e => e.Delivary, "IX_DelivaryProfile").IsUnique();

            entity.Property(e => e.DrivingLicenseNumber)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NationalId)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NationalName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SecondName)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.DelivaryNavigation).WithOne(p => p.DelivaryProfile)
                .HasForeignKey<DelivaryProfile>(d => d.Delivary)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DelivaryProfile_Delivary");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.Party).HasName("PK_Customer");

            entity.ToTable("Delivery");

            entity.Property(e => e.Party).ValueGeneratedNever();

            entity.HasOne(d => d.PartyNavigation).WithOne(p => p.Delivery)
                .HasForeignKey<Delivery>(d => d.Party)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Delivary_Party");
        });

        modelBuilder.Entity<DeliveryActivity>(entity =>
        {
            entity.ToTable("DeliveryActivity");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.DeliveryNavigation).WithMany(p => p.DeliveryActivities)
                .HasForeignKey(d => d.Delivery)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryActivity_Delivery");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.DeliveryActivities)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryActivity_DeliveryActivityType");
        });

        modelBuilder.Entity<DeliveryActivityType>(entity =>
        {
            entity.ToTable("DeliveryActivityType");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DeliveryProfileHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Delivery__3214EC07B1DA8363");

            entity.ToTable("DeliveryProfileHistory");

            entity.Property(e => e.DrivingLicenseExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.DrivingLicenseFileUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.DrivingLicenseNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.LicenseExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.NationalId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.NationalIqamaIdfileUrl)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("NationalIqamaIDFileUrl");
            entity.Property(e => e.NationalName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.ReviewDate).HasColumnType("datetime");
            entity.Property(e => e.ReviewDescription)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.SecondName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SubmitDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VehicleLicenseFileUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.VehicleModel)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VehicleNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VehiclePhotoFileUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.VehicleType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VehicleYear)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DeliveryWallet>(entity =>
        {
            entity.ToTable("DeliveryWallet");

            entity.HasOne(d => d.DeliveryNavigation).WithMany(p => p.DeliveryWallets)
                .HasForeignKey(d => d.Delivery)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryWallet_Delivery");
        });

        modelBuilder.Entity<DiscountCode>(entity =>
        {
            entity.ToTable("DiscountCode");

            entity.HasIndex(e => e.IsActive, "IX_DiscountCode");

            entity.HasIndex(e => e.DiscountType, "IX_DiscountCode_1");

            entity.Property(e => e.Code)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 5)");

            entity.HasOne(d => d.DiscountTypeNavigation).WithMany(p => p.DiscountCodes)
                .HasForeignKey(d => d.DiscountType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountCode_DiscountType");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.ToTable("DiscountType");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentType");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DriverFreeFee>(entity =>
        {
            entity.ToTable("DriverFreeFee");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.ToTable("FAQ");

            entity.Property(e => e.Answer)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Question)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.Faqs)
                .HasForeignKey(d => d.Language)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FAQ_Language");
        });

        modelBuilder.Entity<FavoriteProduct>(entity =>
        {
            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.FavoriteProducts)
                .HasForeignKey(d => d.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FavoriteProducts_Customer");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("Language");

            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location");

            entity.HasIndex(e => e.Party, "IX_Location");

            entity.HasIndex(e => e.IsActive, "IX_Location_1");

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.PartyNavigation).WithMany(p => p.Locations)
                .HasForeignKey(d => d.Party)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Location_Party");
        });

        modelBuilder.Entity<LocationDetail>(entity =>
        {
            entity.HasIndex(e => e.Location, "IX_LocationDetails");

            entity.Property(e => e.ApartmentNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BuildingNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BuildingType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FloorNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StreetNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.LocationNavigation).WithMany(p => p.LocationDetails)
                .HasForeignKey(d => d.Location)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LocationDetails_Location");
        });

        modelBuilder.Entity<MembershipUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AspNetUsers");

            entity.ToTable("MembershipUser");

            entity.HasIndex(e => e.Role, "IX_MembershipUser");

            entity.HasIndex(e => e.IsActive, "IX_MembershipUser_1");

            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Language).HasDefaultValue((byte)1);
            entity.Property(e => e.PasswordHash).IsUnicode(false);

            entity.HasOne(d => d.PartyNavigation).WithMany(p => p.MembershipUsers)
                .HasForeignKey(d => d.Party)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MembershipUser_Party");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.MembershipUsers)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MembershipUser_Role");
        });

        modelBuilder.Entity<MobileAppConfiguration>(entity =>
        {
            entity.ToTable("MobileAppConfiguration");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CustomerAppUrlAndroid).IsUnicode(false);
            entity.Property(e => e.CustomerAppUrlIos).IsUnicode(false);
            entity.Property(e => e.DeliveryAppUrlAndroid).IsUnicode(false);
            entity.Property(e => e.DeliveryAppUrlIos).IsUnicode(false);
            entity.Property(e => e.ProviderAppUrlAndroid).IsUnicode(false);
            entity.Property(e => e.ProviderAppUrlIos).IsUnicode(false);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.Content)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.JsonPayLoad)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ForPartyNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ForParty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_Party");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.HasIndex(e => e.OrderStatus, "IX_Order_4");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DeliveryFee).HasColumnType("money");
            entity.Property(e => e.ItemsPrice).HasColumnType("money");
            entity.Property(e => e.SpecialRequest)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.SystemFee).HasColumnType("money");
            entity.Property(e => e.TotalPrice).HasColumnType("money");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.OrderStatusNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_OrderStatus");

            entity.HasOne(d => d.PaymentTypeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_PaymentType");

            entity.HasMany(d => d.DiscountCodes).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderDiscountCode",
                    r => r.HasOne<DiscountCode>().WithMany()
                        .HasForeignKey("DiscountCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_OrderDiscountCode_DiscountCode"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("Order")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_OrderDiscountCode_Order"),
                    j =>
                    {
                        j.HasKey("Order", "DiscountCode");
                        j.ToTable("OrderDiscountCode");
                        j.HasIndex(new[] { "Order" }, "IX_OrderDiscountCode");
                        j.HasIndex(new[] { "DiscountCode" }, "IX_OrderDiscountCode_1");
                    });
        });

        modelBuilder.Entity<OrderActivity>(entity =>
        {
            entity.ToTable("OrderActivity");

            entity.HasIndex(e => e.Order, "IX_OrderActivity");

            entity.HasIndex(e => e.OrderActivityType, "IX_OrderActivity_1");

            entity.HasIndex(e => e.OperatedBy, "IX_OrderActivity_2");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.OperatedByNavigation).WithMany(p => p.OrderActivities)
                .HasForeignKey(d => d.OperatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderActivity_Party");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.OrderActivities)
                .HasForeignKey(d => d.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderActivity_Order");

            entity.HasOne(d => d.OrderActivityTypeNavigation).WithMany(p => p.OrderActivities)
                .HasForeignKey(d => d.OrderActivityType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderActivity_OrderActivityType");
        });

        modelBuilder.Entity<OrderActivityType>(entity =>
        {
            entity.ToTable("OrderActivityType");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OrderCustomerInformation>(entity =>
        {
            entity.ToTable("OrderCustomerInformation");

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.OrderCustomerInformations)
                .HasForeignKey(d => d.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderCustomerInformation_Customer");

            entity.HasOne(d => d.LocationNavigation).WithMany(p => p.OrderCustomerInformations)
                .HasForeignKey(d => d.Location)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderCustomerInformation_Location");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.OrderCustomerInformations)
                .HasForeignKey(d => d.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderCustomerInformation_Order");
        });

        modelBuilder.Entity<OrderDeliveryInformation>(entity =>
        {
            entity.ToTable("OrderDeliveryInformation");

            entity.HasOne(d => d.DeliveryNavigation).WithMany(p => p.OrderDeliveryInformations)
                .HasForeignKey(d => d.Delivery)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDeliveryInformation_Delivery");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.OrderDeliveryInformations)
                .HasForeignKey(d => d.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDeliveryInformation_Order");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.ToTable("OrderProduct");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProduct_Order");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.Product)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProduct_Product");
        });

        modelBuilder.Entity<OrderProductOffer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_OrderItem");

            entity.ToTable("OrderProductOffer");

            entity.HasIndex(e => e.ProductOffer, "IX_OrderItem");

            entity.HasIndex(e => e.Order, "IX_OrderItem_1");

            entity.Property(e => e.UnitPrice).HasColumnType("money");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.OrderProductOffers)
                .HasForeignKey(d => d.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Order");

            entity.HasOne(d => d.ProductOfferNavigation).WithMany(p => p.OrderProductOffers)
                .HasForeignKey(d => d.ProductOffer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProductOffer_ProductOffer");

            entity.HasMany(d => d.DiscountCodes).WithMany(p => p.OrderItems)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderItemDiscountCode",
                    r => r.HasOne<DiscountCode>().WithMany()
                        .HasForeignKey("DiscountCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_OrderItemDiscountCode_DiscountCode"),
                    l => l.HasOne<OrderProductOffer>().WithMany()
                        .HasForeignKey("OrderItem")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_OrderItemDiscountCode_OrderItem"),
                    j =>
                    {
                        j.HasKey("OrderItem", "DiscountCode");
                        j.ToTable("OrderItemDiscountCode");
                        j.HasIndex(new[] { "OrderItem" }, "IX_OrderItemDiscountCode");
                        j.HasIndex(new[] { "DiscountCode" }, "IX_OrderItemDiscountCode_1");
                    });
        });

        modelBuilder.Entity<OrderProviderInformation>(entity =>
        {
            entity.ToTable("OrderProviderInformation");

            entity.HasOne(d => d.LocationNavigation).WithMany(p => p.OrderProviderInformations)
                .HasForeignKey(d => d.Location)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProviderInformation_Location");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.OrderProviderInformations)
                .HasForeignKey(d => d.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProviderInformation_Order");

            entity.HasOne(d => d.ProviderNavigation).WithMany(p => p.OrderProviderInformations)
                .HasForeignKey(d => d.Provider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProviderInformation_Provider");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("OrderStatus");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Party>(entity =>
        {
            entity.ToTable("Party");

            entity.HasIndex(e => e.IsActive, "IX_Party");

            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.ToTable("PaymentType");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PhoneNumber>(entity =>
        {
            entity.ToTable("PhoneNumber");

            entity.HasIndex(e => e.IsActive, "IX_PhoneNumber");

            entity.HasIndex(e => e.Party, "IX_PhoneNumber_1");

            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.PartyNavigation).WithMany(p => p.PhoneNumbers)
                .HasForeignKey(d => d.Party)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhoneNumber_Party");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductCategory");

            entity.ToTable("Product");

            entity.HasIndex(e => e.IsActive, "IX_Product");

            entity.HasIndex(e => e.ProductCategory, "IX_Product_1");

            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.MeasurementUnit)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.ProductCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_ProductCategory");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductCategory_1");

            entity.ToTable("ProductCategory");

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProductCategoryLocalized>(entity =>
        {
            entity.HasKey(e => new { e.Language, e.ProductCategory });

            entity.ToTable("ProductCategoryLocalized");

            entity.HasIndex(e => e.ProductCategory, "IX_ProductCategoryLocalized");

            entity.HasIndex(e => e.Language, "IX_ProductCategoryLocalized_1");

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.ProductCategoryLocalizeds)
                .HasForeignKey(d => d.Language)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategoryLocalized_Language1");

            entity.HasOne(d => d.ProductCategoryNavigation).WithMany(p => p.ProductCategoryLocalizeds)
                .HasForeignKey(d => d.ProductCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategoryLocalized_ProductCategory1");
        });

        modelBuilder.Entity<ProductDiscount>(entity =>
        {
            entity.ToTable("ProductDiscount");

            entity.HasIndex(e => e.IsActive, "IX_ProductDiscount");

            entity.HasIndex(e => e.Product, "IX_ProductDiscount_1");

            entity.HasIndex(e => e.DiscountCode, "IX_ProductDiscount_2");

            entity.Property(e => e.AssignedDate).HasColumnType("datetime");

            entity.HasOne(d => d.DiscountCodeNavigation).WithMany(p => p.ProductDiscounts)
                .HasForeignKey(d => d.DiscountCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductDiscount_DiscountCode");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.ProductDiscounts)
                .HasForeignKey(d => d.Product)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductDiscount_Product");
        });

        modelBuilder.Entity<ProductLocalized>(entity =>
        {
            entity.HasKey(e => new { e.Language, e.Product });

            entity.ToTable("ProductLocalized");

            entity.HasIndex(e => e.Product, "IX_ProductLocalized");

            entity.HasIndex(e => e.Language, "IX_ProductLocalized_1");

            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.MeasurementUnit)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.ProductLocalizeds)
                .HasForeignKey(d => d.Language)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductLocalized_Language");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.ProductLocalizeds)
                .HasForeignKey(d => d.Product)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductLocalized_Product");
        });

        modelBuilder.Entity<ProductOffer>(entity =>
        {
            entity.ToTable("ProductOffer");

            entity.HasIndex(e => e.IsActive, "IX_ProductOffer");

            entity.HasIndex(e => e.Product, "IX_ProductOffer_1");

            entity.HasIndex(e => e.Provider, "IX_ProductOffer_2");

            entity.Property(e => e.FromDate).HasColumnType("datetime");
            entity.Property(e => e.OffPercentage).HasColumnType("decimal(10, 5)");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ToDate).HasColumnType("datetime");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.ProductOffers)
                .HasForeignKey(d => d.Product)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOffer_Product");

            entity.HasOne(d => d.ProviderNavigation).WithMany(p => p.ProductOffers)
                .HasForeignKey(d => d.Provider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOffer_Provider");
        });

        modelBuilder.Entity<ProductOfferDiscount>(entity =>
        {
            entity.ToTable("ProductOfferDiscount");

            entity.HasIndex(e => e.IsActive, "IX_ProductOfferDiscount");

            entity.HasIndex(e => e.DiscountCode, "IX_ProductOfferDiscount_1");

            entity.HasIndex(e => e.ProductOffer, "IX_ProductOfferDiscount_2");

            entity.Property(e => e.AssignedDate).HasColumnType("datetime");

            entity.HasOne(d => d.DiscountCodeNavigation).WithMany(p => p.ProductOfferDiscounts)
                .HasForeignKey(d => d.DiscountCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOfferDiscount_DiscountCode");

            entity.HasOne(d => d.ProductOfferNavigation).WithMany(p => p.ProductOfferDiscounts)
                .HasForeignKey(d => d.ProductOffer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductOfferDiscount_ProductOffer");
        });

        modelBuilder.Entity<ProductRequest>(entity =>
        {
            entity.ToTable("ProductRequest");

            entity.HasIndex(e => e.Status, "IX_ProductRequest");

            entity.HasIndex(e => e.ProductCategory, "IX_ProductRequest_1");

            entity.HasIndex(e => e.Provider, "IX_ProductRequest_2");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.OffPercentage).HasColumnType("decimal(10, 5)");
            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.ProductCategoryNavigation).WithMany(p => p.ProductRequests)
                .HasForeignKey(d => d.ProductCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRequest_ProductCategory");

            entity.HasOne(d => d.ProviderNavigation).WithMany(p => p.ProductRequests)
                .HasForeignKey(d => d.Provider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRequest_Provider");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.ProductRequests)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRequest_ProductRequestStatus");
        });

        modelBuilder.Entity<ProductRequestDataLocalized>(entity =>
        {
            entity.HasKey(e => new { e.ProductRequest, e.Language }).HasName("PK_ProductRequestDataLocalized_1");

            entity.ToTable("ProductRequestDataLocalized");

            entity.HasIndex(e => e.Language, "IX_ProductRequestDataLocalized");

            entity.HasIndex(e => e.ProductRequest, "IX_ProductRequestDataLocalized_1");

            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.MeasurementUnit)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.ProductRequestDataLocalizeds)
                .HasForeignKey(d => d.Language)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRequestDataLocalized_Language");

            entity.HasOne(d => d.ProductRequestNavigation).WithMany(p => p.ProductRequestDataLocalizeds)
                .HasForeignKey(d => d.ProductRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRequestDataLocalized_ProductRequest");
        });

        modelBuilder.Entity<ProductRequestStatus>(entity =>
        {
            entity.ToTable("ProductRequestStatus");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.Party);

            entity.ToTable("Provider");

            entity.Property(e => e.Party).ValueGeneratedNever();

            entity.HasOne(d => d.PartyNavigation).WithOne(p => p.Provider)
                .HasForeignKey<Provider>(d => d.Party)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Provider_Party");
        });

        modelBuilder.Entity<ProviderActivity>(entity =>
        {
            entity.ToTable("ProviderActivity");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.ProviderNavigation).WithMany(p => p.ProviderActivities)
                .HasForeignKey(d => d.Provider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderActivity_Provider");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.ProviderActivities)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderActivity_DeliveryActivityType");
        });

        modelBuilder.Entity<ProviderDocument>(entity =>
        {
            entity.ToTable("ProviderDocument");

            entity.HasIndex(e => e.Provider, "IX_ProviderDocument");

            entity.HasIndex(e => e.DocumentType, "IX_ProviderDocument_1");

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.DocumentTypeNavigation).WithMany(p => p.ProviderDocuments)
                .HasForeignKey(d => d.DocumentType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderDocument_DocumentType");

            entity.HasOne(d => d.ProviderNavigation).WithMany(p => p.ProviderDocuments)
                .HasForeignKey(d => d.Provider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderDocument_Provider");
        });

        modelBuilder.Entity<ProviderProfile>(entity =>
        {
            entity.ToTable("ProviderProfile");

            entity.HasIndex(e => e.Provider, "IX_ProviderProfile").IsUnique();

            entity.Property(e => e.CompanyName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NationalApprovalId)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.OwnerName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.OwnerNationalId)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SecondName)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.ProviderNavigation).WithOne(p => p.ProviderProfile)
                .HasForeignKey<ProviderProfile>(d => d.Provider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderProfile_Provider");
        });

        modelBuilder.Entity<ProviderProfileHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Provider__3214EC076F2F8415");

            entity.ToTable("ProviderProfileHistory");

            entity.Property(e => e.ApartmentNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BuildingNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BuildingType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FloorNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LocationName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NationalApprovalId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OwnerName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.OwnerNationalApprovalFileUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.OwnerNationalId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OwnershipDocumentFileUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ReviewDate).HasColumnType("datetime");
            entity.Property(e => e.ReviewDescription)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.SecondName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StreetNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SubmitDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<ProviderWallet>(entity =>
        {
            entity.ToTable("ProviderWallet");

            entity.HasOne(d => d.ProviderNavigation).WithMany(p => p.ProviderWallets)
                .HasForeignKey(d => d.Provider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderWallet_Provider");
        });

        modelBuilder.Entity<ResetPasswordCode>(entity =>
        {
            entity.ToTable("ResetPasswordCode");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.MembershipUserNavigation).WithMany(p => p.ResetPasswordCodes)
                .HasForeignKey(d => d.MembershipUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResetPasswordCode_MembershipUser");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.ToTable("SearchHistory");

            entity.Property(e => e.Term)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.SearchHistories)
                .HasForeignKey(d => d.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SearchHistory_Party");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.ExpireDate).HasColumnType("datetime");
            entity.Property(e => e.Token).IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sessions_MembershipUser");
        });

        modelBuilder.Entity<SliderItem>(entity =>
        {
            entity.ToTable("SliderItem");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.TargetNavigation).WithMany(p => p.InverseTargetNavigation)
                .HasForeignKey(d => d.Target)
                .HasConstraintName("FK_SliderItem_SliderItem");
        });

        modelBuilder.Entity<SystemActivity>(entity =>
        {
            entity.ToTable("SystemActivity");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.SystemActivities)
                .HasForeignKey(d => d.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemActivity_Order");
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MaxMinutesToCancelOrderWaitingAprovider).HasColumnName("MaxMinutesToCancelOrderWaitingAProvider");
        });

        modelBuilder.Entity<TextualConfig>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Language });

            entity.Property(e => e.Value).IsUnicode(false);

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.TextualConfigs)
                .HasForeignKey(d => d.Language)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TextualConfigs_Language");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicle");

            entity.HasIndex(e => e.Delivary, "IX_Vehicle");

            entity.Property(e => e.Model)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Number)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Year)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.DelivaryNavigation).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.Delivary)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_Delivary");
        });

        modelBuilder.Entity<VerificationToken>(entity =>
        {
            entity.ToTable("VerificationToken");

            entity.HasIndex(e => e.MembershipUser, "IX_VerificationToken");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MembershipUserNavigation).WithMany(p => p.VerificationTokens)
                .HasForeignKey(d => d.MembershipUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VerificationToken_MembershipUser");
        });

        modelBuilder.Entity<Withdraw>(entity =>
        {
            entity.ToTable("Withdraw");

            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IbanNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IBanNumber");

            entity.HasOne(d => d.PartyNavigation).WithMany(p => p.Withdraws)
                .HasForeignKey(d => d.Party)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Withdraw_Party");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
