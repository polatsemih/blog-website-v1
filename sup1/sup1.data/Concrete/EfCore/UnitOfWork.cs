using System.Threading.Tasks;
using sup1.data.Abstract;

namespace sup1.data.Concrete.EfCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SupContext _context;
        public UnitOfWork(SupContext context)
        {
            _context = context;
        }

        private EfCoreArticleRepository _articleRepository;
        private EfCoreArticleContentImageRepository _articlecontentimageRepository;
        private EfCoreCategoryRepository _categoryRepository;
        private EfCoreContactMessageRepository _contactmessageRepository;
        private EfCoreCommentRepository _commentRepository;
        private EfCoreCommentReplyRepository _commentrepliesRepository;

        public IArticleRepository Articles => _articleRepository = _articleRepository ?? new EfCoreArticleRepository(_context);
        public IArticleContentImageRepository ArticleContentImages => _articlecontentimageRepository = _articlecontentimageRepository ?? new EfCoreArticleContentImageRepository(_context);
        public ICategoryRepository Categories => _categoryRepository = _categoryRepository ?? new EfCoreCategoryRepository(_context);
        public IContactMessageRepository ContactMessages => _contactmessageRepository = _contactmessageRepository ?? new EfCoreContactMessageRepository(_context);
        public ICommentRepository Comments => _commentRepository = _commentRepository ?? new EfCoreCommentRepository(_context);
        public ICommentReplyRepository CommentReplies => _commentrepliesRepository = _commentrepliesRepository ?? new EfCoreCommentReplyRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}