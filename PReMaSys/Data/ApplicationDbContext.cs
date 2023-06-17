using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PReMaSys.Data;
using PReMaSys.Models;
using System.Reflection.Emit;
using System.Linq;

namespace PReMaSys.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<DomainAccount> DomainAccount { get; set; }
        public DbSet<Rewards> Rewards { get; set; }
        public DbSet<AccLogin> AccLogin { get; set; }
        public DbSet<SalesEmployeeRecord> SalesEmployeeRecords { get; set; }
        public DbSet<AdminRoles> AdminRoles { get; set; }
        public DbSet<AccLoginSE> AccLoginSE { get; set; }

        public DbSet<AddToCart> AddToCart { get; set; }

        public DbSet<Purchase> Purchase { get; set; }

        public DbSet<SERecord> SERecord { get; set; }

        public DbSet<AuditLogEntry> AuditLogs { get; set; }

        public DbSet<SalesPerformance> SalesPerformances { get; set; }
        public DbSet<SalesForecast> SalesForecasts { get; set; }




        //This Code block enables developer to custom/rename identity tables in sql
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SalesPerformance>()
            .HasOne(s => s.SalesForecast)
            .WithOne(f => f.SalesPerformance)
            .HasForeignKey<SalesForecast>(f => f.SPID);

            base.OnModelCreating(builder);

            RenameIdentityTables(builder);
        }

        protected void RenameIdentityTables(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("prms");
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }

    }
}
