using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.business.Abstract;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.business.Concrete
{
    public class CommentReplyManager : ICommentReplyService
    {
        private readonly IUnitOfWork _unitofwork;
        public CommentReplyManager(IUnitOfWork unitofwork)
        {
            this._unitofwork = unitofwork;
        }

        public List<CommentReply> GetCommentRepliesByCommentId(int id)
        {
            return _unitofwork.CommentReplies.GetCommentRepliesByCommentId(id);
        }

        public List<CommentReply> GetCommentReplies(int page, int pageSize)
        {
            return _unitofwork.CommentReplies.GetCommentReplies(page, pageSize);
        }

        public int GetCommentRepliesCount()
        {
            return _unitofwork.CommentReplies.GetCommentRepliesCount();
        }

        public async Task<List<CommentReply>> GetAll()
        {
            return await _unitofwork.CommentReplies.GetAll();
        }

        public async Task<CommentReply> GetById(int id)
        {
            return await _unitofwork.CommentReplies.GetById(id);
        }

        public void Create(CommentReply entity)
        {
            _unitofwork.CommentReplies.Create(entity);
            _unitofwork.Save();
        }

        public void Update(CommentReply entity)
        {
            _unitofwork.CommentReplies.Update(entity);
            _unitofwork.Save();
        }

        public void Delete(CommentReply entity)
        {
            _unitofwork.CommentReplies.Delete(entity);
            _unitofwork.Save();
        }       
    }
}