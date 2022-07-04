using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.entity;

namespace sup1.business.Abstract
{
    public interface ICommentService
    {
        List<Comment> GetByArticleId(int id);
        List<Comment> GetComments(int page, int pageSize);
        int GetCommentsCount();
        Task<List<Comment>> GetAll();
        Task<Comment> GetById(int id);
        void Create(Comment entity);
        void Update(Comment entity);
        void Delete(Comment entity);
    }
}