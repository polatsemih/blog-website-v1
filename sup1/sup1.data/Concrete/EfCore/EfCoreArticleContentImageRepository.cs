using sup1.data.Abstract;
using sup1.entity;

namespace sup1.data.Concrete.EfCore
{
    public class EfCoreArticleContentImageRepository : EfCoreGenericRepository<ArticleContentImage>, IArticleContentImageRepository
    {
        public EfCoreArticleContentImageRepository(SupContext context) : base(context)
        {
            
        }
        private SupContext SupContext
        {
            get { return context as SupContext; }
        }
    }
}