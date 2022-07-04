using System;
using System.Threading.Tasks;

namespace sup1.data.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IArticleRepository Articles { get; }
        IArticleContentImageRepository ArticleContentImages { get; }
        ICategoryRepository Categories { get; }
        IContactMessageRepository ContactMessages { get; }
        ICommentRepository Comments { get; }
        ICommentReplyRepository CommentReplies { get; }
        void Save();
        Task<int> SaveAsync();
    }
}