using Microsoft.EntityFrameworkCore;
using sup1.data.Configurations;
using sup1.entity;

namespace sup1.data.Concrete.EfCore
{
    public class SupContext : DbContext
    {
        public SupContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleContentImage> ArticleContentImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReply> CommentReplies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ArticleConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleContentImageConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ContactMessageConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new CommentReplyConfiguration());
            modelBuilder.Seed();
        }
    }
}