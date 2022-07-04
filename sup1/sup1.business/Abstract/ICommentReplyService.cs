using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.entity;

namespace sup1.business.Abstract
{
    public interface ICommentReplyService
    {
        List<CommentReply> GetCommentRepliesByCommentId(int id);
        List<CommentReply> GetCommentReplies(int page, int pageSize);
        int GetCommentRepliesCount();
        Task<List<CommentReply>> GetAll();
        Task<CommentReply> GetById(int id);
        void Create(CommentReply entity);
        void Update(CommentReply entity);
        void Delete(CommentReply entity);
    }
}