using System.Collections.Generic;
using sup1.entity;

namespace sup1.data.Abstract
{
    public interface ICommentRepository : IRepository<Comment>
    {
        List<Comment> GetByArticleId(int id);
        List<Comment> GetComments(int page, int pageSize);
        int GetCommentsCount();
    }
}