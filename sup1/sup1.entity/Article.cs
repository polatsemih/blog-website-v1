using System;
using System.Collections.Generic;

namespace sup1.entity
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Explanation { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateEdited { get; set; }
        public string ImageUrl { get; set; }
        public bool IsHome { get; set; }
        public string ArticleContent { get; set; }
        public int ArticleViewCount { get; set; }
        
        public Category Category { get; set; }
        public int? CategoryId { get; set; }
        
        public List<Comment> Comments { get; set; }

        public List<ArticleContentImage> ArticleContentImages { get; set; }
    }
}