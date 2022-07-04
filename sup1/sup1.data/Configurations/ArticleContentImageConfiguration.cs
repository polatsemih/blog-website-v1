using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sup1.entity;

namespace sup1.data.Configurations
{
    public class ArticleContentImageConfiguration : IEntityTypeConfiguration<ArticleContentImage>
    {
        public void Configure(EntityTypeBuilder<ArticleContentImage> builder)
        {
            builder.HasKey(b => b.ArticleContentImageId);
            builder.Property(b => b.Name).IsRequired();
            builder.Property(b => b.DateAdded).HasDefaultValueSql("getdate()");
            builder.HasOne(p => p.Article)
                    .WithMany(b => b.ArticleContentImages)
                    .HasForeignKey(p => p.ArticleId)
                    .HasConstraintName("ForeignKey_ArticleContentImage")
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}