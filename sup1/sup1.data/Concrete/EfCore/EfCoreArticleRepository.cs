using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.data.Concrete.EfCore
{
    public class EfCoreArticleRepository : EfCoreGenericRepository<Article>, IArticleRepository
    {
        public EfCoreArticleRepository(SupContext context) : base(context)
        {
            
        }
        private SupContext SupContext
        {
            get { return context as SupContext; }
        }

        public List<Article> GetArticlesBySearchResult(string searchString, int page, int pageSize)
        {
            var articles = SupContext.Articles
                            .Where(i => i.Title.ToLower().Contains(searchString) || i.Explanation.ToLower().Contains(searchString))
                            .OrderBy(i => i.DateAdded);
                            
            return articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public Article GetByUrl(string articleUrl)
        {
            return SupContext.Articles
                    .Include(i => i.Comments)
                    .Where(i => i.Url == articleUrl)
                    .FirstOrDefault();
        }

        public List<Article> GetHomePageArticles(int page, int pageSize)
        {
            var articles = SupContext.Articles
                            .Where(i => i.IsHome)
                            .ToList();

            return articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        
        public Article GetArticleByIdWithCategory(int id)
        {
            return SupContext.Articles
                            .Include(i => i.Category)
                            .Where(i => i.ArticleId == id)
                            .FirstOrDefault();
        }

        public Article GetArticleByIdWithArticleContentImage(int id)
        {
            return SupContext.Articles
                            .Include(i => i.ArticleContentImages)
                            .Where(i => i.ArticleId == id)
                            .FirstOrDefault();
        }

        public List<Article> GetArticles(int page, int pageSize)
        {
            var articles = SupContext.Articles
                            .OrderBy(d => d.DateAdded)
                            .ToList();

            return articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<Article> GetArticlesByCategory(string name, int page, int pageSize)
        {
            var articles = SupContext.Articles
                                    .Include(i => i.Category)
                                    .Where(a => a.Category.Url == name)
                                    .OrderBy(i => i.DateAdded);

            return articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<Article> GetArticlesByCategory(string name)
        {
            var articles = SupContext.Articles
                                    .Include(i => i.Category)
                                    .Where(a => a.Category.Url == name)
                                    .OrderByDescending(i => i.DateAdded);

            return articles.ToList();
        }

        public List<Article> GetArticlesByCategoryId(int id)
        {
            var articles = SupContext.Articles
                                    .Include(i => i.Category)
                                    .Where(a => a.Category.CategoryId == id)
                                    .OrderBy(i => i.DateAdded);

            return articles.ToList();
        }

        public int GetArticlesCountBySearchResult(string searchString)
        {
            var articles = SupContext.Articles
                                    .Where(i => i.Title.ToLower().Contains(searchString) || i.Explanation.ToLower().Contains(searchString));
            
            return articles.Count();
        }

        public int GetHomePageArticlesCount()
        {
            var aticles = SupContext.Articles
                            .Where(i => i.IsHome)
                            .ToList();

            return aticles.Count();
        }

        public int GetArticlesCount()
        {
            var articles = SupContext.Articles
                            .ToList();

            return articles.Count();
        }

        public int GetArticlesCountByCategory(string category)
        {
            return SupContext.Articles
                            .Where(i => i.Category.Url == category)
                            .Count();
        }
    }
}