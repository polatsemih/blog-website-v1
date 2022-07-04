using Microsoft.EntityFrameworkCore;
using sup1.entity;

namespace sup1.data.Configurations
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Article> builder)
        {
            builder.HasKey(b => b.ArticleId);
            builder.Property(b => b.Author).IsRequired().HasMaxLength(20);
            builder.Property(b => b.Title).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Explanation).IsRequired().HasMaxLength(100);
            builder.Property(b => b.DateAdded).HasDefaultValueSql("getdate()");
            builder.Property(b => b.DateEdited).HasDefaultValueSql("getdate()");
            builder.Property(b => b.ImageUrl).IsRequired().HasMaxLength(100);
            builder.HasOne(p => p.Category)
                    .WithMany(b => b.Articles)
                    .HasForeignKey(p => p.CategoryId)
                    .HasConstraintName("ForeignKey_Article_Category")
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}