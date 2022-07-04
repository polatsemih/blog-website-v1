using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.entity;

namespace sup1.business.Abstract
{
    public interface IArticleService
    {
        List<Article> GetArticlesBySearchResult(string searchString, int page, int pageSize);
        Article GetByUrl(string articleUrl);
        List<Article> GetHomePageArticles(int page, int pageSize);
        Article GetArticleByIdWithCategory(int id);
        Article GetArticleByIdWithArticleContentImage(int id);
        List<Article> GetArticles(int page, int pageSize);
        List<Article> GetArticlesByCategory(string name, int page, int pageSize);
        List<Article> GetArticlesByCategory(string name);
        List<Article> GetArticlesByCategoryId(int id);
        int GetArticlesCountBySearchResult(string searchString);
        int GetHomePageArticlesCount();
        int GetArticlesCount();
        int GetArticlesCountByCategory(string category);
        Task<List<Article>> GetAll();
        Task<Article> GetById(int id);
        void Create(Article entity);
        void Update(Article entity);
        void Delete(Article entity);
    }
}