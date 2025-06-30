using CashFlow.IdentityAndAccess.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CashFlow.IdentityAndAccess.Infrastructure.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Role
            builder.Entity<Role>(e =>
            {
                e.ToTable("role");
                e.HasKey(r => r.Id);
                e.Property(r => r.Id).HasColumnName("role_id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(r => r.Name).HasColumnName("role_name").IsRequired().HasMaxLength(50);
                e.HasIndex(r => r.Name).IsUnique();
                e.Property(r => r.Description).HasColumnName("role_desc").HasMaxLength(200);
            });

            // User
            builder.Entity<AppUser>(e =>
            {
                e.ToTable("app_user");
                e.HasKey(u => u.Id);
                e.Property(u => u.Id).HasColumnName("app_user_id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(u => u.LoginName).HasColumnName("app_user_login_name").IsRequired().HasMaxLength(360);
                e.Property(u => u.Email).HasColumnName("app_user_email").IsRequired().HasMaxLength(360);
                e.Property(u => u.PasswordHash).HasColumnName("app_user_pwd").IsRequired().HasMaxLength(200);
                e.Property(u => u.Enabled).HasColumnName("app_user_enabled");
                e.HasIndex(u => u.LoginName).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
            });

            // UserRole
            builder.Entity<UserRole>(e =>
            {
                e.ToTable("user_role");
                e.HasKey(ur => new { ur.UserId, ur.RoleId });
                e.Property(ur => ur.UserId).HasColumnName("app_user_id");
                e.Property(ur => ur.RoleId).HasColumnName("role_id");
                e.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
                e.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
            });

            // RefreshToken
            builder.Entity<RefreshToken>(e =>
            {
                e.ToTable("refresh_token");
                e.HasKey(r => r.Id);
                e.Property(r => r.Id).HasColumnName("refresh_token_id").HasDefaultValueSql("gen_random_uuid()");
                e.Property(r => r.UserId).HasColumnName("app_user_id");
                e.Property(r => r.Token).HasColumnName("refresh_token_content").IsRequired().HasMaxLength(200);
                e.Property(r => r.Expiry).HasColumnName("refresh_token_expiry");
                e.Property(r => r.IsRevoked).HasColumnName("refresh_token_is_revoked");
                e.Property(r => r.CreatedAt).HasColumnName("created_at");
                e.Property(r => r.RevokedAt).HasColumnName("revoked_at");
                e.HasIndex(r => r.Token).IsUnique();
            });

            // Roles
            builder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("3d6f97b5-f1c2-4bbf-9210-31d7f79e9bba"), Name = "ADMIN", Description = "Full system access" },
                new Role { Id = Guid.Parse("b2621c5c-7bfa-4b97-a7d3-25417c320f5c"), Name = "USER", Description = "General user" },
                new Role { Id = Guid.Parse("d416a013-5648-4e81-9e14-5be6ebfa323b"), Name = "MANAGER", Description = "Management user" }
            );
        }
    }
}