using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sup1.entity;

namespace sup1.data.Configurations
{
    public class CommentReplyConfiguration : IEntityTypeConfiguration<CommentReply>
    {
        public void Configure(EntityTypeBuilder<CommentReply> builder)
        {
            builder.HasKey(b => b.CommentReplyId);
            builder.Property(b => b.Message).IsRequired();
            builder.Property(b => b.DateAdded).HasDefaultValueSql("getdate()");
            builder.HasOne(p => p.Comment)
                    .WithMany(b => b.CommentReplys)
                    .HasForeignKey(p => p.CommentId)
                    .HasConstraintName("ForeignKey_CommentReply_Comment")
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}