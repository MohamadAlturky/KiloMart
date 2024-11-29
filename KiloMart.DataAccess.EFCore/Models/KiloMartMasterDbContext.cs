using Microsoft.EntityFrameworkCore;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class KiloMartMasterDbContext : DbContext
{
    public KiloMartMasterDbContext()
    {
    }

    public KiloMartMasterDbContext(DbContextOptions<KiloMartMasterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppSetting> AppSettings { get; set; }


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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
