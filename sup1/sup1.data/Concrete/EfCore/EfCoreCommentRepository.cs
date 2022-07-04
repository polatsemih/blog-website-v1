using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.data.Concrete.EfCore
{
    public class EfCoreCommentRepository : EfCoreGenericRepository<Comment>, ICommentRepository
    {
        public EfCoreCommentRepository(SupContext context) : base(context)
        {
            
        }
        private SupContext SupContext
        {
            get { return context as SupContext; }
        }

        public List<Comment> GetByArticleId(int id)
        {
            var comments = SupContext.Comments
                                        .Where(w => w.ArticleId == id)
                                        .ToList();

            return comments;
        }

        public List<Comment> GetComments(int page, int pageSize)
        {
            var comments = SupContext.Comments
                                    .OrderBy(d => d.DateAdded)
                                    .ToList();

            return comments.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetCommentsCount()
        {
            var comments = SupContext.Comments
                                    .ToList();

            return comments.Count();
        }
    }
}