using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sup1.business.Abstract;
using sup1.entity;
using sup1.webui.Identity;
using sup1.webui.Models;

namespace sup1.webui.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<User> _userManager;
        private IArticleService _articleService;
        private ICommentService _commentService;
        private ICommentReplyService _commentreplyService;
        public HomeController(UserManager<User> userManager, IArticleService articleService, ICommentService commentService, ICommentReplyService commentreplyService)
        {
            this._userManager = userManager;
            this._articleService = articleService;
            this._commentService = commentService;
            this._commentreplyService = commentreplyService;
        }

        [Route("/Home/HandleError/{code:int}")]
        public IActionResult HandleError(int code)
        {
            ViewData["ErrorMessage"] = "Something went wrong.";
            return View("~/Views/Shared/_HandleError.cshtml");
        }

        [HttpGet]
        public IActionResult ArticleSearch(string s, int page = 1)
        {
            if (string.IsNullOrEmpty(s))
            {
                return RedirectToAction("Article");
            }

            var searchstring = System.Web.HttpUtility.HtmlEncode(s.Trim().ToLower());

            const int pageSize = 4;
            var articleViewModel = new ArticleListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _articleService.GetArticlesCountBySearchResult(searchstring),
                    ItemsPerPage = pageSize,
                    CurrentPage = page,
                    CurrentSearch = searchstring
                },
                Articles = _articleService.GetArticlesBySearchResult(searchstring, page, pageSize)
            };
            return View(articleViewModel);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ARTICLE-PART
        [HttpGet]
        public IActionResult Article(int page = 1)
        {
            const int pageSize = 4;
            var articles = _articleService.GetHomePageArticles(page, pageSize);

            var articleListViewModel = new ArticleListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _articleService.GetHomePageArticlesCount(),
                    ItemsPerPage = pageSize,
                    CurrentPage = page
                },
                Articles = articles
            };
            return View(articleListViewModel);
        }

        [HttpGet]
        public IActionResult ArticleCategory(string category, int page = 1)
        {
            const int pageSize = 4;
            var articles = _articleService.GetArticlesByCategory(category, page, pageSize);

            var articleListViewModel = new ArticleListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _articleService.GetArticlesCountByCategory(category),
                    ItemsPerPage = pageSize,
                    CurrentPage = page,
                    CurrentCategory = category
                },
                Articles = articles
            };
            return View(articleListViewModel);
        }
    
        [HttpGet]
        public IActionResult ArticleDetail(string articleUrl)
        {
            var article = _articleService.GetByUrl(articleUrl);
            if (article == null)
            {
                return NotFound();
            }

            article.ArticleViewCount += 1;
            _articleService.Update(article);

            var comments = _commentService.GetByArticleId(article.ArticleId);
            
            List<CommentReply> commentreplies = new List<CommentReply>();
            foreach (var c in comments)
            {
                var commentreply = _commentreplyService.GetCommentRepliesByCommentId(c.CommentId);
                commentreplies.AddRange(commentreply);
            }

            var articledetail = new ArticleCommentModel()
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Url = article.Url,
                Explanation = article.Explanation,
                ArticleContent = article.ArticleContent,
                ArticleViewCount = article.ArticleViewCount,
                ImageUrl = article.ImageUrl,
                ArticleDateAdded = article.DateAdded,
                ArticleDateEdited = article.DateEdited,
                Comments = comments,
                CommentReplies = commentreplies
            };

            var samecategoryarticle = _articleService.GetArticlesByCategoryId((int)article.CategoryId);
            ViewBag.SameCategoryArticles = samecategoryarticle;
            return View(articledetail);
        }
    }
}