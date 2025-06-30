using CashFlow.Entries.Domain.Entries.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Entries.Infrastructure.Data
{
    public class EntriesDbContext : DbContext
    {
        public DbSet<Entry> Entries => Set<Entry>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<EntryType> EntryTypes => Set<EntryType>();

        public EntriesDbContext(DbContextOptions<EntriesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // EntryType
            modelBuilder.Entity<EntryType>(et =>
            {
                et.ToTable("entry_type");
                et.HasKey(x => x.Id);
                et.Property(x => x.Id)
                    .HasColumnName("entry_type_id")
                    .HasDefaultValueSql("gen_random_uuid()");
                et.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("entry_type_name");
            });

            // Category
            modelBuilder.Entity<Category>(c =>
            {
                c.ToTable("category");
                c.HasKey(x => x.Id);
                c.Property(x => x.Id)
                    .HasColumnName("category_id")
                    .HasDefaultValueSql("gen_random_uuid()");
                c.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("category_name");

                // FK
                c.Property(x => x.EntryTypeId)
                    .IsRequired()
                    .HasColumnName("entry_type_id");
                c.HasOne(x => x.EntryType)
                    .WithMany(et => et.Categories)
                    .HasForeignKey(x => x.EntryTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Entry
            modelBuilder.Entity<Entry>(e =>
            {
                e.ToTable("entry");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id)
                    .HasColumnName("entry_id")
                    .HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.Amount)
                    .IsRequired()
                    .HasColumnName("entry_amount");
                e.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("entry_description");
                e.Property(x => x.CategoryId)
                    .IsRequired()
                    .HasColumnName("category_id");
                e.Property(x => x.EntryTypeId)
                    .IsRequired()
                    .HasColumnName("entry_type_id");
                e.Property(x => x.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by");
                e.Property(x => x.CreatedAt)
                    .IsRequired()
                    .HasColumnName("created_at");
                e.Property(x => x.ModifiedBy)
                    .HasColumnName("modified_by");
                e.Property(x => x.ModifiedAt)
                    .HasColumnName("modified_at");

                e.HasOne(x => x.Category)
                    .WithMany()
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.EntryType)
                    .WithMany()
                    .HasForeignKey(x => x.EntryTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seeds EntryType
            var creditTypeId = Guid.Parse("a7964402-ab2b-4a56-9670-72f723364f80");
            var debitTypeId = Guid.Parse("880fd242-952b-4b4a-82ca-9a32a2e3866c");
            modelBuilder.Entity<EntryType>().HasData(
                new EntryType(creditTypeId, "CREDIT"),
                new EntryType(debitTypeId, "DEBIT")
            );

            // Seeds Category
            modelBuilder.Entity<Category>().HasData(
                new Category(Guid.Parse("cbb3f1ea-91cb-42e8-8daf-f68bfa520c2f"), "REVENUE", creditTypeId),
                new Category(Guid.Parse("f841be5a-9923-419b-87a2-c3654e02620a"), "OTHER_INCOME", creditTypeId),
                new Category(Guid.Parse("fdaacfad-b3c9-4c5a-9c3b-3ce9667fabed"), "INVESTMENTS", creditTypeId),
                new Category(Guid.Parse("929ed0ff-d817-4a82-8124-3dfcd6a1f9ce"), "GIFTS_DONATIONS", creditTypeId),
                new Category(Guid.Parse("b0c380a7-8472-4e45-a04d-5f9920b2877a"), "LOAN_RECEIVED", creditTypeId),
                new Category(Guid.Parse("63eb96fe-2c91-43b9-b415-c65ee867e2bd"), "OTHER_CREDITS", creditTypeId),
                new Category(Guid.Parse("283b1aca-ef7a-4fce-bcbb-8752e2153c08"), "FOOD", debitTypeId),
                new Category(Guid.Parse("a11d43e6-96d6-415f-b2fe-6f0a55970eb9"), "TRANSPORTATION", debitTypeId),
                new Category(Guid.Parse("f38043ac-9294-4909-af22-e260f9e871bb"), "EDUCATION", debitTypeId),
                new Category(Guid.Parse("56e7533b-4e48-4483-b387-4aee71e454bd"), "HEALTH", debitTypeId),
                new Category(Guid.Parse("47b934b2-e7ec-4dd9-b89e-f542b37505ca"), "LEISURE", debitTypeId),
                new Category(Guid.Parse("96d8efc8-61f1-4007-9917-0862b0f2f57a"), "HOUSING", debitTypeId),
                new Category(Guid.Parse("c5617a27-be6e-4848-82e8-bbf0f6548097"), "COMMUNICATION", debitTypeId),
                new Category(Guid.Parse("b1ac6638-318d-423f-8c4c-fa3f614fd2e1"), "SUPPLIES", debitTypeId),
                new Category(Guid.Parse("054a551e-9841-4bef-ad35-8bb9a381a916"), "UTILITIES", debitTypeId),
                new Category(Guid.Parse("c87bfe80-d970-410f-b503-5355c5a372d3"), "MAINTENANCE", debitTypeId),
                new Category(Guid.Parse("adc25139-e6f6-432a-879b-1f05e6bd32b8"), "EQUIPMENT", debitTypeId),
                new Category(Guid.Parse("d371511b-f8d7-4468-8e9f-d540e7c0d7b6"), "FEES", debitTypeId),
                new Category(Guid.Parse("0c90948f-f1db-42cb-8b6c-5c43cfa99d41"), "CLOTHING", debitTypeId),
                new Category(Guid.Parse("a604b4c1-637c-454a-b4a1-e4a5c1af0c18"), "OTHER_DEBITS", creditTypeId)
            );
        }
    }
}