using System.Collections.Generic;
using System.Linq;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.data.Concrete.EfCore
{
    public class EfCoreCommentReplyRepository : EfCoreGenericRepository<CommentReply>, ICommentReplyRepository
    {
        public EfCoreCommentReplyRepository(SupContext context) : base(context)
        {
            
        }
        private SupContext SupContext
        {
            get { return context as SupContext; }
        }

        public List<CommentReply> GetCommentRepliesByCommentId(int id)
        {
            var commentreplies = SupContext.CommentReplies
                                        .Where(w => w.CommentId == id)
                                        .OrderBy(o => o.DateAdded)
                                        .ToList();

            return commentreplies;
        }

        public List<CommentReply> GetCommentReplies(int page, int pageSize)
        {
            var commentreplies = SupContext.CommentReplies
                                    .OrderBy(d => d.DateAdded)
                                    .ToList();

            return commentreplies.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetCommentRepliesCount()
        {
            var commentreplies = SupContext.CommentReplies
                                    .ToList();

            return commentreplies.Count();
        }
    }
}