using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using sup1.entity;

namespace sup1.webui.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter the category name.")]
        [StringLength(50)]
        public string Name { get; set; }  
        public List<Article> Articles { get; set; }
    }
}