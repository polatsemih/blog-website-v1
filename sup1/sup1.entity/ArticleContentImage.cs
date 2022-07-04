using System;

namespace sup1.entity
{
    public class ArticleContentImage
    {
        public int ArticleContentImageId { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public Article Article { get; set; }
        public int? ArticleId { get; set; }
    }
}