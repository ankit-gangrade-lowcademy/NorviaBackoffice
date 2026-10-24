using Microsoft.EntityFrameworkCore;
using NorviaBackoffice.Models.Entities;
namespace NorviaBackoffice.Data;
public sealed class NorviaDbContext(DbContextOptions<NorviaDbContext> options) : DbContext(options)
{
    public DbSet<NorviaUser> NorviaUsers => Set<NorviaUser>();
    public DbSet<NorviaRole> NorviaRoles => Set<NorviaRole>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserExtended> UserExtended => Set<UserExtended>();
    public DbSet<NorviaProduct> NorviaProducts => Set<NorviaProduct>();
    public DbSet<ProductUserRole> ProductUserRoles => Set<ProductUserRole>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NorviaRole>(e => { e.ToTable("NorviaRoles", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(50).IsRequired(); e.Property(x => x.Description).HasMaxLength(255); });
        modelBuilder.Entity<NorviaUser>(e => { e.ToTable("NorviaUsers", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(150).IsRequired(); e.Property(x => x.Email).HasMaxLength(320).IsRequired(); e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired(); e.Property(x => x.LastLoginOn).HasPrecision(3); e.Property(x => x.CreatedOn).HasPrecision(3); e.Property(x => x.UpdatedOn).HasPrecision(3); e.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId); });
        modelBuilder.Entity<Tenant>(e => { e.ToTable("Tenant", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(255).IsRequired(); });
        modelBuilder.Entity<NorviaProduct>(e => { e.ToTable("NorviaProducts", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).HasColumnName("ID"); e.Property(x => x.Name).HasMaxLength(255).IsRequired(); });
        modelBuilder.Entity<ProductUserRole>(e => { e.ToTable("ProductUserRole", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).HasColumnName("ID"); e.Property(x => x.ProductId).HasColumnName("ProductID"); e.Property(x => x.ProductRoleId).HasColumnName("ProductRoleID"); e.Property(x => x.Name).HasMaxLength(255).IsRequired(); e.Property(x => x.Order).HasColumnName("Order"); e.HasOne(x => x.Product).WithMany(x => x.Roles).HasForeignKey(x => x.ProductId); });
        modelBuilder.Entity<UserExtended>(e => { e.ToTable("UserExtended", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.OutSystemsUserId).HasColumnName("OutSystemsUserId").HasMaxLength(255); e.Property(x => x.TenantId).HasColumnName("Tenant_ID"); e.Property(x => x.Name).HasMaxLength(150); e.Property(x => x.Surname).HasMaxLength(150); e.Property(x => x.UserInternalId).HasColumnName("UserInternalID").HasMaxLength(100); e.Property(x => x.PhoneNumber).HasMaxLength(30); e.Property(x => x.Email).HasMaxLength(320); e.Property(x => x.UserRoleId).HasColumnName("UserRoleID"); e.Property(x => x.Address).HasMaxLength(1000); e.Property(x => x.CreatedOn).HasPrecision(3); e.Property(x => x.UpdatedOn).HasPrecision(3); e.HasOne(x => x.Tenant).WithMany(x => x.Users).HasForeignKey(x => x.TenantId); });
    }
}
