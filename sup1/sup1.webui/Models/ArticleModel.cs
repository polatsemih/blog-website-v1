using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using sup1.entity;

namespace sup1.webui.Models
{
    public class ArticleModel
    {
        [Required(ErrorMessage = "Please enter the article title")]
        [MaxLength(100,ErrorMessage ="Max lenght is 100 word")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter the article explanation")]
        [MaxLength(100,ErrorMessage ="Max lenght is 100 word")]
        public string Explanation { get; set; }
        public string ArticleContent { get; set; }
        public bool IsHome { get; set; }
        public string ImageUrl { get; set; }
        public int ArticleId { get; set; }
        public Category SelectedCategory { get; set; }
    }

    public class ArticleContentModel
    {
        public int ArticleId { get; set; }
        
        [Required(ErrorMessage = "Please enter the article content")]
        public string ArticleContent { get; set; }
    }

    public class ArticleListViewModel
    {
        public PageInfo PageInfo { get; set; }
        public List<Article> Articles { get; set; }
    }

    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }
        public string CurrentSearch { get; set; }
        public int TotalPages()
        {
            return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        }
    }

    public class ArticleCommentModel
    {
        public string UserId { get; set; }
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Explanation { get; set; }
        public string ArticleContent { get; set; }
        public int ArticleViewCount { get; set; }
        public DateTime ArticleDateAdded { get; set; }
        public DateTime ArticleDateEdited { get; set; }
        public string ImageUrl { get; set; }
        public List<Comment> Comments { get; set; }
        public List<CommentReply> CommentReplies { get; set; }
        public string CommentMessage { get; set; }
        public string CommentReplyMessage { get; set; }
        public int CommentId { get; set; }
        public int CommentReplyId { get; set; }
    }
}