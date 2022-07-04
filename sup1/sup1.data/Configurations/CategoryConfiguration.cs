using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sup1.entity;

namespace sup1.data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(m => m.CategoryId);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Url).IsRequired().HasMaxLength(50);
            builder.Property(m => m.DateAdded).HasDefaultValueSql("getdate()");
        }
    }
}