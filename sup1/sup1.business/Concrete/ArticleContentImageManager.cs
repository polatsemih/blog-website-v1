using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.business.Abstract;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.business.Concrete
{
    public class ArticleContentImageManager : IArticleContentImageService
    {
        private readonly IUnitOfWork _unitofwork;
        public ArticleContentImageManager(IUnitOfWork unitofwork)
        {
            this._unitofwork = unitofwork;
        }

        public async Task<List<ArticleContentImage>> GetAll()
        {
            return await _unitofwork.ArticleContentImages.GetAll();
        }

        public async Task<ArticleContentImage> GetById(int id)
        {
            return await _unitofwork.ArticleContentImages.GetById(id);
        }

        public void Create(ArticleContentImage entity)
        {
            _unitofwork.ArticleContentImages.Create(entity);
            _unitofwork.Save();
        }

        public void Update(ArticleContentImage entity)
        {
            _unitofwork.ArticleContentImages.Update(entity);
            _unitofwork.Save();
        }
        
        public void Delete(ArticleContentImage entity)
        {
            _unitofwork.ArticleContentImages.Delete(entity);
            _unitofwork.Save();
        }
    }
}