using sup1.entity;

namespace sup1.data.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Category GetCategoryByIdWithArticles(int categoryId);
    }
}