using Microsoft.EntityFrameworkCore;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;

namespace LuarcaDynamicPortfolio.Mvc.Data
{
    public class PersonalWebsiteDbContext : DbContext
    {
        public PersonalWebsiteDbContext(DbContextOptions<PersonalWebsiteDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<PersonalDetail> PersonalDetails { get; set; } = null!;
        public DbSet<Education> Education { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<Hobby> Hobbies { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;

        // Rubric Dynamic Website Content Tables
        public DbSet<CompanyInfo> CompanyInfo { get; set; } = null!;
        public DbSet<ProductService> ProductServices { get; set; } = null!;
        public DbSet<ContactInfo> ContactInfo { get; set; } = null!;
        public DbSet<OwnerInfo> OwnerInfo { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table mappings to existing SQL Server schema
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
                entity.Property(e => e.PasswordSalt).HasMaxLength(256).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(150);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.AuditLogId);
                entity.Property(e => e.ActionType).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ActionDetail).HasMaxLength(500);
                entity.Property(e => e.IPAddress).HasMaxLength(50);
                entity.Property(e => e.ActionDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PersonalDetail>(entity =>
            {
                entity.ToTable("PersonalDetails");
                entity.HasKey(e => e.PersonalDetailId);
                entity.Property(e => e.FullName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.ContactNumber).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.ProfileImagePath).HasMaxLength(300);
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.LastModifiedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.LastModifiedBy)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Education>(entity =>
            {
                entity.ToTable("Education");
                entity.HasKey(e => e.EducationId);
                entity.Property(e => e.Level).HasMaxLength(100).IsRequired();
                entity.Property(e => e.SchoolName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Course).HasMaxLength(200);
                entity.Property(e => e.YearTo).HasMaxLength(20);
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skills");
                entity.HasKey(e => e.SkillId);
                entity.Property(e => e.SkillName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Hobby>(entity =>
            {
                entity.ToTable("Hobbies");
                entity.HasKey(e => e.HobbyId);
                entity.Property(e => e.HobbyName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");
                entity.HasKey(e => e.ProjectId);
                entity.Property(e => e.Title).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
                entity.Property(e => e.TechTags).HasMaxLength(300);
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Rubric Content Entities
            modelBuilder.Entity<CompanyInfo>(entity =>
            {
                entity.ToTable("CompanyInfo");
                entity.HasKey(e => e.CompanyInfoId);
                entity.Property(e => e.CompanyName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Tagline).HasMaxLength(250);
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<ProductService>(entity =>
            {
                entity.ToTable("ProductServices");
                entity.HasKey(e => e.ProductServiceId);
                entity.Property(e => e.Title).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Tags).HasMaxLength(300);
                entity.Property(e => e.ActionUrl).HasMaxLength(300);
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<ContactInfo>(entity =>
            {
                entity.ToTable("ContactInfo");
                entity.HasKey(e => e.ContactInfoId);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.ContactNumber).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.OfficeHours).HasMaxLength(150);
                entity.Property(e => e.SocialLinks).HasMaxLength(300);
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<OwnerInfo>(entity =>
            {
                entity.ToTable("OwnerInfo");
                entity.HasKey(e => e.OwnerInfoId);
                entity.Property(e => e.FullName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.TitleRole).HasMaxLength(150);
                entity.Property(e => e.ProfileImagePath).HasMaxLength(300);
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
