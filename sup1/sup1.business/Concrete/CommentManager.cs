using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.business.Abstract;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.business.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly IUnitOfWork _unitofwork;
        public CommentManager(IUnitOfWork unitofwork)
        {
            this._unitofwork = unitofwork;
        }

        public List<Comment> GetByArticleId(int id)
        {
            return _unitofwork.Comments.GetByArticleId(id);
        }

        public List<Comment> GetComments(int page, int pageSize)
        {
            return _unitofwork.Comments.GetComments(page, pageSize);
        }

        public int GetCommentsCount()
        {
            return _unitofwork.Comments.GetCommentsCount();
        }

        public async Task<List<Comment>> GetAll()
        {
            return await _unitofwork.Comments.GetAll();
        }

        public async Task<Comment> GetById(int id)
        {
            return await _unitofwork.Comments.GetById(id);
        }

        public void Create(Comment entity)
        {
            _unitofwork.Comments.Create(entity);
            _unitofwork.Save();
        }

        public void Update(Comment entity)
        {
            _unitofwork.Comments.Update(entity);
            _unitofwork.Save();
        }

        public void Delete(Comment entity)
        {
            _unitofwork.Comments.Delete(entity);
            _unitofwork.Save();
        }       
    }
}