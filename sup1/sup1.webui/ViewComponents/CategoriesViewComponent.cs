using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sup1.business.Abstract;

namespace sup1.webui.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private ICategoryService _categoryService;
        public CategoriesViewComponent(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (RouteData.Values["category"] != null)
            {
                ViewBag.SelectedCategory = RouteData?.Values["category"];
            }
                
            return View(await _categoryService.GetAll());
        }
    }
}