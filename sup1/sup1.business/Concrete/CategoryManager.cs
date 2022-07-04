using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.business.Abstract;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly IUnitOfWork _unitofwork;
        public CategoryManager(IUnitOfWork unitofwork)
        {
            this._unitofwork = unitofwork;
        }

        public Category GetCategoryByIdWithArticles(int categoryId)
        {
            return _unitofwork.Categories.GetCategoryByIdWithArticles(categoryId);
        }

        public async Task<List<Category>> GetAll()
        {
            return await _unitofwork.Categories.GetAll();
        }

        public async Task<Category> GetById(int id)
        {
            return await _unitofwork.Categories.GetById(id);
        }

        public void Create(Category entity)
        {
            _unitofwork.Categories.Create(entity);
            _unitofwork.Save();
        }

        public void Update(Category entity)
        {
            _unitofwork.Categories.Update(entity);
            _unitofwork.Save();
        }
        
        public void Delete(Category entity)
        {
            _unitofwork.Categories.Delete(entity);
            _unitofwork.Save();
        }
    }
}