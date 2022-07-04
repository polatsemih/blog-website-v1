using System;
using System.Collections.Generic;

namespace sup1.entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public List<Article> Articles { get; set; }
    }
}