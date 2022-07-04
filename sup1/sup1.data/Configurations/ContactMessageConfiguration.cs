using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sup1.entity;

namespace sup1.data.Configurations
{
    public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.HasKey(m => m.MessageId);
            builder.Property(m => m.Subject).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Message).IsRequired();
            builder.Property(b => b.DateAdded).HasDefaultValueSql("getdate()");
            builder.Property(u => u.UserId).HasMaxLength(50);
        }
    }
}