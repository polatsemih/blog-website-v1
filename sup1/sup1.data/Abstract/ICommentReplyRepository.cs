using System.Collections.Generic;
using sup1.entity;

namespace sup1.data.Abstract
{
    public interface ICommentReplyRepository : IRepository<CommentReply>
    {
        List<CommentReply> GetCommentRepliesByCommentId(int id);
        List<CommentReply> GetCommentReplies(int page, int pageSize);
        int GetCommentRepliesCount();
    }
}