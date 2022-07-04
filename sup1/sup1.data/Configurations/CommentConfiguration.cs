using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sup1.entity;

namespace sup1.data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(b => b.CommentId);
            builder.Property(b => b.Message).IsRequired();
            builder.Property(b => b.DateAdded).HasDefaultValueSql("getdate()");
            builder.HasOne(p => p.Article)
                    .WithMany(b => b.Comments)
                    .HasForeignKey(p => p.ArticleId)
                    .HasConstraintName("ForeignKey_Comment_Article")
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}