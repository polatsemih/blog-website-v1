using Microsoft.EntityFrameworkCore;
using sup1.entity;

namespace sup1.data.Configurations
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasData(
                new Article() { ArticleId = 1, Author = "Utku_POLAT", Title = "Title-1", Explanation = "Explanation-1", ImageUrl = "article.png", IsHome = true, ArticleContent = "Article-1", CategoryId = 1 }
            );

            modelBuilder.Entity<ArticleContentImage>().HasData(
                new ArticleContentImage() { ArticleContentImageId = 1, Name = "article-content-image.png", ArticleId = 1 }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category() { CategoryId = 1, Name = "Html", Url = "html" }
            );

            modelBuilder.Entity<ContactMessage>().HasData(
                new ContactMessage() { MessageId = 1, Subject = "Subject-1", Message = "Message-1", UserId = "1" }
            );

            modelBuilder.Entity<Comment>().HasData(
                new Comment() { CommentId = 1, Message = "Comment-1", UserId = "1", ArticleId = 1 }
            );

            modelBuilder.Entity<CommentReply>().HasData(
                new CommentReply() { CommentReplyId = 1, Message = "CommentReply-1", UserId = "1", CommentId = 1 }
            );
        }
    }
}