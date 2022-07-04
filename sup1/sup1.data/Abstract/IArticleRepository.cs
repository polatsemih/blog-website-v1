using System.Collections.Generic;
using sup1.entity;

namespace sup1.data.Abstract
{
    public interface IArticleRepository : IRepository<Article>
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
    }
}