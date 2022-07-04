using System.Linq;
using Microsoft.EntityFrameworkCore;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.data.Concrete.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category>, ICategoryRepository
    { 
        public EfCoreCategoryRepository(SupContext context) : base(context)
        {

        }
        private SupContext SupContext
        {
            get { return context as SupContext; }
        }

        public Category GetCategoryByIdWithArticles(int categoryId)
        {
            return SupContext.Categories
                            .Where(i => i.CategoryId == categoryId)
                            .Include(i => i.Articles)
                            .FirstOrDefault();
        }
    }
}