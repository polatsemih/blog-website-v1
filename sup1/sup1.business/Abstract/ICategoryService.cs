using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.entity;

namespace sup1.business.Abstract
{
    public interface ICategoryService
    {
        Category GetCategoryByIdWithArticles(int categoryId);
        Task<List<Category>> GetAll();
        Task<Category> GetById(int id);
        void Create(Category entity);
        void Update(Category entity);
        void Delete(Category entity);
    }
}