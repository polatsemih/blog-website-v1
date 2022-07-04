using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.business.Abstract;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.business.Concrete
{
    public class ArticleManager : IArticleService
    {
        private readonly IUnitOfWork _unitofwork;
        public ArticleManager(IUnitOfWork unitofwork)
        {
            this._unitofwork = unitofwork;
        }

        public List<Article> GetArticlesBySearchResult(string searchString, int page, int pageSize)
        {
            return _unitofwork.Articles.GetArticlesBySearchResult(searchString, page, pageSize);
        }

        public Article GetByUrl(string articleUrl)
        {
            return _unitofwork.Articles.GetByUrl(articleUrl);
        }

        public List<Article> GetHomePageArticles(int page, int pageSize)
        {
            return _unitofwork.Articles.GetHomePageArticles(page, pageSize);
        }

        public Article GetArticleByIdWithCategory(int id)
        {
            return _unitofwork.Articles.GetArticleByIdWithCategory(id);
        }

        public Article GetArticleByIdWithArticleContentImage(int id)
        {
            return _unitofwork.Articles.GetArticleByIdWithArticleContentImage(id);
        }

        public List<Article> GetArticles(int page, int pageSize)
        {
            return _unitofwork.Articles.GetArticles(page, pageSize);
        }

        public List<Article> GetArticlesByCategory(string name, int page, int pageSize)
        {
            return _unitofwork.Articles.GetArticlesByCategory(name, page, pageSize);
        }

        public List<Article> GetArticlesByCategory(string name)
        {
            return _unitofwork.Articles.GetArticlesByCategory(name);
        }

        public List<Article> GetArticlesByCategoryId(int id)
        {
            return _unitofwork.Articles.GetArticlesByCategoryId(id);
        }

        public int GetArticlesCountBySearchResult(string searchString)
        {
            return _unitofwork.Articles.GetArticlesCountBySearchResult(searchString);
        }

        public int GetHomePageArticlesCount()
        {
            return _unitofwork.Articles.GetHomePageArticlesCount();
        }

        public int GetArticlesCount()
        {
            return _unitofwork.Articles.GetArticlesCount();
        }

        public int GetArticlesCountByCategory(string category)
        {
            return _unitofwork.Articles.GetArticlesCountByCategory(category);
        }

        public async Task<List<Article>> GetAll()
        {
            return await _unitofwork.Articles.GetAll();
        }

        public async Task<Article> GetById(int id)
        {
            return await _unitofwork.Articles.GetById(id);
        }

        public void Create(Article entity)
        {
            _unitofwork.Articles.Create(entity);
            _unitofwork.Save();
        }

        public void Update(Article entity)
        {
            _unitofwork.Articles.Update(entity);
            _unitofwork.Save();
        }
        
        public void Delete(Article entity)
        {
            _unitofwork.Articles.Delete(entity);
            _unitofwork.Save();
        }
    }
}