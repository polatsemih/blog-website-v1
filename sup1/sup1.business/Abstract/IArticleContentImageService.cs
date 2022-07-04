using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.entity;

namespace sup1.business.Abstract
{
    public interface IArticleContentImageService
    {
        Task<List<ArticleContentImage>> GetAll();
        Task<ArticleContentImage> GetById(int id);
        void Create(ArticleContentImage entity);
        void Update(ArticleContentImage entity);
        void Delete(ArticleContentImage entity);
    }
}