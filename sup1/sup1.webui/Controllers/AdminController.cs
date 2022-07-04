using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sup1.business.Abstract;
using sup1.entity;
using sup1.webui.Extensions;
using sup1.webui.Identity;
using sup1.webui.Models;
using sup1.webui.Models.Account;
using sup1.webui.Models.Admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace sup1.webui.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private IArticleService _articleService;
        private IArticleContentImageService _articlecontentimageService;
        private ICategoryService _categoryService;
        private IContactMessageService _contactmessageService;
        private ICommentService _commentService;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public AdminController(IArticleService articlService, IArticleContentImageService articlecontentimageService, ICategoryService categoryService, IContactMessageService contactmessageService, ICommentService commentService, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._articleService = articlService;
            this._articlecontentimageService = articlecontentimageService;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._categoryService = categoryService;
            this._contactmessageService = contactmessageService;
            this._commentService = commentService;
        }

        // ARTICLE-PART
        [HttpGet]
        public IActionResult ArticleAdminList(int page = 1)
        {
            const int pageSize = 4;

            var articles = _articleService.GetHomePageArticles(page, pageSize);

            return View(new ArticleListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _articleService.GetHomePageArticlesCount(),
                    ItemsPerPage = pageSize,
                    CurrentPage = page
                },
                Articles = articles
            });
        }

        [HttpGet]
        public IActionResult ArticleCategoryAdmin(string category, int page = 1)
        {
            const int pageSize = 4;
            var articles = _articleService.GetArticlesByCategory(category, page, pageSize);

            return View(new ArticleListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _articleService.GetArticlesCountByCategory(category),
                    ItemsPerPage = pageSize,
                    CurrentPage = page,
                    CurrentCategory = category
                },
                Articles = articles
            });
        }

        [HttpGet]
        public async Task<IActionResult> ArticleCreate()
        {
            ViewBag.Categories = await _categoryService.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleCreate(ArticleModel model, int categoryId, IFormFile file)
        {
            var allCategories = await _categoryService.GetAll();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = allCategories;
                return View(model);
            }

            if (categoryId == 0)
            {
                ModelState.AddModelError("IsHome", "You must select one category for the article.");
                ViewBag.Categories = allCategories;
                return View(model);
            }
            else if (file == null)
            {
                ModelState.AddModelError("ImageUrl", "Please upload a article image.");
                ViewBag.Categories = allCategories;
                return View(model);
            }
            else if (file.Length > (1024 * 1024 * 1))
            {
                ModelState.AddModelError("ImageUrl", "Please upload a photo less than 1mb in size.");
                ViewBag.Categories = allCategories;
                return View(model);
            }
            else if (string.IsNullOrEmpty(model.Explanation) || string.IsNullOrEmpty(model.Title))
            {
                ModelState.AddModelError("Explanation", "Please enter all fields valid.");
                ViewBag.Categories = allCategories;
                return View(model);
            }
            else
            {
                // Insertig new blog image
                var randomnumber = new Random().Next(99, 999999999);
                var filename = System.Web.HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(file.FileName));
                var randomName = string.Format($"{filename}{randomnumber}{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}");
                model.ImageUrl = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\article-image", randomName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var newarticle = new Article()
                {
                    Author = User.Claims.ElementAt(1).Value,
                    Title = System.Web.HttpUtility.HtmlEncode(model.Title.Trim()),
                    Url = System.Web.HttpUtility.HtmlEncode(model.Title.Trim().ToLower().Replace(' ','-')),
                    Explanation = System.Web.HttpUtility.HtmlEncode(model.Explanation.Trim()),
                    ImageUrl = model.ImageUrl,
                    IsHome = model.IsHome,
                    CategoryId = categoryId
                };

                _articleService.Create(newarticle);

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Article Created",
                    Message = $"Time to set article content",
                    AlertType = "success"
                });
                return RedirectToAction("ArticleContentCreate", "Admin", new { articleId = newarticle.ArticleId});
            }
        }

        [HttpGet]
        public async Task<IActionResult> ArticleContentCreate(int articleId)
        {
            var article = await _articleService.GetById(articleId);
            if (article == null)
            {
                return NotFound();
            }

            return View(new ArticleContentModel()
            {
                ArticleId = article.ArticleId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleContentCreate(ArticleContentModel model)
        {
            if (string.IsNullOrEmpty(model.ArticleContent))
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Input",
                    Message = $"Please enter the content valid",
                    AlertType = "success"
                });
                return View(model);
            }

            var article = await _articleService.GetById(model.ArticleId);

            article.ArticleContent = model.ArticleContent.Trim();

            _articleService.Update(article);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Article Created With Content",
                Message = $"{article.Title}",
                AlertType = "success"
            });
            return RedirectToAction("ArticleAdminList");
        }

        [HttpGet]
        public async Task<IActionResult> ArticleEdit(int articleId)
        {
            var article = _articleService.GetArticleByIdWithCategory(articleId);
            if (article == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _categoryService.GetAll();
            return View(new ArticleModel()
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Explanation = article.Explanation,
                ImageUrl = article.ImageUrl,
                IsHome = article.IsHome,
                SelectedCategory = article.Category
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleEdit(ArticleModel model, int categoryId, IFormFile file)
        {
            var allCategories = await _categoryService.GetAll();

            if (!ModelState.IsValid)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Input",
                    Message = "Please enter all fields valid",
                    AlertType = "danger"
                });
                ViewBag.Categories = allCategories;
                return Redirect("" + model.ArticleId);      
            }

            var article = await _articleService.GetById(model.ArticleId);
            if (article == null)
            {
                return NotFound();
            }
            else if (article.Author != User.Claims.ElementAt(1).Value)
            {
                return NotFound();
            }
            else if (categoryId == 0)
            {
                ModelState.AddModelError("IsHome", "You must select one category for the article.");
                ViewBag.Categories = allCategories;
                return Redirect("" + model.ArticleId);
            }
            else if (string.IsNullOrEmpty(model.Explanation) || string.IsNullOrEmpty(model.Title))
            {
                ModelState.AddModelError("Explanation", "Please enter all fields valid.");
                ViewBag.Categories = allCategories;
                return Redirect("" + model.ArticleId);
            }
            else if (file == null)
            {
                article.Title = System.Web.HttpUtility.HtmlEncode(model.Title.Trim());
                article.Url = System.Web.HttpUtility.HtmlEncode(model.Title.Trim().ToLower().Replace(' ','-'));
                article.Explanation = System.Web.HttpUtility.HtmlEncode(model.Explanation.Trim());
                article.IsHome = model.IsHome;
                article.CategoryId = categoryId;
                article.DateEdited = DateTime.Now;

                _articleService.Update(article);

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Article Edited",
                    Message = $"Time to edit article content",
                    AlertType = "success"
                });
                return RedirectToAction("ArticleContentEdit", "Admin", new { articleId = article.ArticleId});
            }
            else if (file.Length > (1024 * 1024 * 1))
            {
                ModelState.AddModelError("ImageUrl", "Please upload a photo less than 1mb in size.");
                ViewBag.Categories = allCategories;
                return View(model);
            }
            else
            {
                // Deleting old article image
                var pathCurrent = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\article-image", model.ImageUrl);
                FileInfo fileCurrent = new FileInfo(pathCurrent);
                fileCurrent.Delete();
                
                // Insertig new article image
                var randomnumber = new Random().Next(99, 999999999);
                var filename = System.Web.HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(file.FileName));
                var randomName = string.Format($"{filename}{randomnumber}{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}");
                model.ImageUrl = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\article-image", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                article.Title = System.Web.HttpUtility.HtmlEncode(model.Title.Trim());
                article.Url = System.Web.HttpUtility.HtmlEncode(model.Title.Trim().ToLower().Replace(' ','-'));
                article.Explanation = System.Web.HttpUtility.HtmlEncode(model.Explanation.Trim());
                article.ImageUrl = model.ImageUrl;
                article.IsHome = model.IsHome;
                article.CategoryId = categoryId;

                _articleService.Update(article);

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Article Edited",
                    Message = $"Time to edit article content",
                    AlertType = "success"
                });
                return RedirectToAction("ArticleContentEdit", "Admin", new { articleId = article.ArticleId});
            }      
        }

        [HttpGet]
        public async Task<IActionResult> ArticleContentEdit(int articleId)
        {
            var article = await _articleService.GetById(articleId);
            if (article == null)
            {
                return NotFound();
            }

            return View(new ArticleContentModel()
            {
                ArticleId = article.ArticleId,
                ArticleContent = article.ArticleContent
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleContentEdit(ArticleContentModel model)
        {
            if (string.IsNullOrEmpty(model.ArticleContent))
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Input",
                    Message = $"Please enter the content valid",
                    AlertType = "success"
                });
                return View(model);
            }

            var article = await _articleService.GetById(model.ArticleId);

            article.ArticleContent = model.ArticleContent.Trim();

            _articleService.Update(article);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Article Edited With Content",
                Message = $"{article.Title}",
                AlertType = "success"
            });
            return RedirectToAction("ArticleAdminList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ArticleDelete(int articleId)
        {
            var article = _articleService.GetArticleByIdWithArticleContentImage(articleId);
            if (article == null)
            {
                return NotFound();
            }

            if (article.Author != User.Claims.ElementAt(1).Value)
            {
                return NotFound();  
            }

            // Deleting old article image
            var articleContentImages = article.ArticleContentImages.Select(i => i.Name);
            if (articleContentImages.Count() > 0)
            {
                foreach (var item in articleContentImages)
                {
                    var pathCurrent2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\article-image\\article-content-images", item);
                    FileInfo fileCurrent2 = new FileInfo(pathCurrent2);
                    fileCurrent2.Delete();
                }
            }

            var pathCurrent = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\article-image", article.ImageUrl);
            FileInfo fileCurrent = new FileInfo(pathCurrent);
            fileCurrent.Delete();

            _articleService.Delete(article);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Article Deleted",
                Message = $"{article.Title}",
                AlertType = "warning"
            });
            return RedirectToAction("ArticleAdminList");
        }
    
        [HttpPost]
        public async Task<IActionResult> ArticleUploadImage(IFormFile file, int articleId)
        {
            // Insertig new article image
            var randomnumber = new Random().Next(99, 999999999);
            var filename = System.Web.HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(file.FileName));
            var uploadedArticleContentImage = string.Format($"{filename}-article-content-image-{randomnumber}{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\article-image\\article-content-images", uploadedArticleContentImage);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var articleContentImage = new ArticleContentImage() {
                Name = uploadedArticleContentImage,
                ArticleId = articleId
            };

            _articlecontentimageService.Create(articleContentImage);

            return new JsonResult(new { location = "/folder/images/article-image/article-content-images/" + uploadedArticleContentImage });
        }
        // CATEGORY-PART
        public async Task<IActionResult> CategoryList()
        {
            var categories = await _categoryService.GetAll();
            ViewBag.CategoriesCount = categories.Count();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpGet]
        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryCreate(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Please enter category name.");
                return View(model);
            }

            var entity = new Category()
            {
                Name = System.Web.HttpUtility.HtmlEncode(model.Name.Trim()),
                Url = System.Web.HttpUtility.HtmlEncode(model.Name.Trim().ToLower().Replace(' ', '-'))
            };
            _categoryService.Create(entity);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Category Created",
                Message = $"{entity.Name}",
                AlertType = "success"
            });
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public IActionResult CategoryEdit(int categoryId)
        {
            var entity = _categoryService.GetCategoryByIdWithArticles(categoryId);
            if (entity == null)
            {
                return NotFound();
            }

            return View(new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Articles = entity.Articles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryEdit(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);             
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Please enter category name.");
                return View(model);
            }

            var entity = await _categoryService.GetById(model.CategoryId);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = System.Web.HttpUtility.HtmlEncode(model.Name.Trim());
            entity.Url = System.Web.HttpUtility.HtmlEncode(model.Name.Trim().ToLower().Replace(' ', '-'));

            _categoryService.Update(entity);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Category Edited",
                Message = $"{entity.Name}",
                AlertType = "success"
            });
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var entity = await _categoryService.GetById(categoryId);
            if (entity == null)
            {
                return NotFound();
            }

            _categoryService.Delete(entity);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Category Deleted",
                Message = $"{entity.Name}",
                AlertType = "warning"
            });
            return RedirectToAction("CategoryList");            
        }

        // USER-PART
        [HttpGet]
        public IActionResult UserList()
        {
            return View(_userManager.Users.OrderBy(i => i.UserName));
        }

        [HttpGet]
        public async Task<IActionResult> UserDetail(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserDetailsModel()
            {
                UserId = UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImageUrl = user.ImageUrl
            });
        }

        [HttpGet]
        public IActionResult UserCreate()
        {
            var roles = _roleManager.Roles.Select(r => r.Name);
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate(UserCreateModel model, string[] selectedRoles)
        {
            var roles = _roleManager.Roles.Select(r => r.Name);

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = roles;
                return View(model);
            }
            
            var users = _userManager.Users;
            foreach (var item in users)
            {
                if (item.UserName == model.UserName)
                {
                    ModelState.AddModelError("UserName", "This username is already taken.");
                    ViewBag.Roles = roles;
                    return View(model);
                }

                if (item.Email == model.Email)
                {
                    ModelState.AddModelError("Email", "This email already in use");
                    ViewBag.Roles = roles;
                    return View(model);
                }
            }

            var newuser = new User()
            {
                FirstName = System.Web.HttpUtility.HtmlEncode(model.FirstName.Trim()),
                LastName = System.Web.HttpUtility.HtmlEncode(model.LastName.Trim()),
                UserName = System.Web.HttpUtility.HtmlEncode(model.UserName.Trim()),
                Email = System.Web.HttpUtility.HtmlEncode(model.Email),
                EmailConfirmed = model.EmailConfirmed,
                ImageUrl = "user-logo.png"
            };

            var result = await _userManager.CreateAsync(newuser, model.Password);
            if (result.Succeeded)
            {
                if (selectedRoles.Count() == 0)
                {
                    await _userManager.AddToRoleAsync(newuser, "user");
                }

                selectedRoles = selectedRoles ?? new string[] { };
                await _userManager.AddToRolesAsync(newuser, selectedRoles);

                TempData.Put("message", new AlertMessage()
                {
                    Title = "User Created",
                    Message = $"{model.UserName}",
                    AlertType = "success"
                });
                return RedirectToAction("UserList");
            }
            ModelState.AddModelError("RePassword", "Please enter all values valid");
            ViewBag.Roles = roles;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            var selectedRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.Select(r => r.Name);
            ViewBag.Roles = roles;

            return View(new UserDetailsModel()
            {
                UserId = UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImageUrl = user.ImageUrl,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                SelectedRoles = selectedRoles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] selectedRoles, IFormFile file)
        {
            var roles = _roleManager.Roles.Select(i => i.Name);
            selectedRoles = selectedRoles ?? new string[] { };

            if (!ModelState.IsValid)
            {
                model.SelectedRoles = selectedRoles;
                ViewBag.Roles = roles;
                return View(model);
            }

            var selecteduser = await _userManager.FindByIdAsync(model.UserId);
            if (selecteduser == null)
            {
                return NotFound();
            }

            if (model.UserName.Trim() != selecteduser.UserName)
            {
                foreach (var item in _userManager.Users)
                {
                    if (item.UserName == model.UserName.Trim())
                    {
                        model.SelectedRoles = selectedRoles;
                        ViewBag.Roles = roles;
                        ModelState.AddModelError("UserName","This user name is already taken");
                        return View(model);
                    }
                }
            }

            if (await _userManager.IsInRoleAsync(selecteduser, "admin"))
            {
                if (model.UserName != null)
                {
                    var articles = await _articleService.GetAll();
                    foreach (var article in articles)
                    {
                        if (article.Author == selecteduser.UserName)
                        {
                            article.Author = System.Web.HttpUtility.HtmlEncode(model.UserName.Trim());
                            _articleService.Update(article);
                        }
                    }
                }
            }

            var userRoles = await _userManager.GetRolesAsync(selecteduser);
            await _userManager.AddToRolesAsync(selecteduser, selectedRoles.Except(userRoles));
            await _userManager.RemoveFromRolesAsync(selecteduser, userRoles.Except(selectedRoles));

            if (selectedRoles.Count() == 0)
            {
                await _userManager.AddToRoleAsync(selecteduser, "user");
            }

            if (file == null)
            {
                selecteduser.FirstName = System.Web.HttpUtility.HtmlEncode(model.FirstName.Trim());
                selecteduser.LastName = System.Web.HttpUtility.HtmlEncode(model.LastName.Trim());
                selecteduser.UserName = System.Web.HttpUtility.HtmlEncode(model.UserName.Trim());
                selecteduser.Email = System.Web.HttpUtility.HtmlEncode(model.Email);
                selecteduser.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(selecteduser);
                if (result.Succeeded)
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "User Edited",
                        Message = $"{model.UserName}",
                        AlertType = "success"
                    });
                    return RedirectToAction("UserList");
                }

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Error",
                    Message = "Something went wrong",
                    AlertType = "danger"
                });
                return RedirectToAction("UserList"); 
            }
            else
            {
                if (file.Length < (1024 * 1024 * 1))
                {
                    if (selecteduser.ImageUrl != "user-logo.png")
                    {
                        // Deleting old product image
                        var pathCurrent = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\user-image", selecteduser.ImageUrl);
                        FileInfo fileCurrent = new FileInfo(pathCurrent);
                        fileCurrent.Delete();
                    }

                    // Insertig new product image
                    var randomnumber = new Random().Next(99, 999999999);
                    var filename = System.Web.HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(file.FileName));
                    var randomName = string.Format($"{filename}{randomnumber}{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}");
                    selecteduser.ImageUrl = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\user-image", randomName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    selecteduser.FirstName = System.Web.HttpUtility.HtmlEncode(model.FirstName.Trim());
                    selecteduser.LastName = System.Web.HttpUtility.HtmlEncode(model.LastName.Trim());
                    selecteduser.UserName = System.Web.HttpUtility.HtmlEncode(model.UserName.Trim());
                    selecteduser.Email = System.Web.HttpUtility.HtmlEncode(model.Email);
                    selecteduser.EmailConfirmed = model.EmailConfirmed;
                    
                    var result = await _userManager.UpdateAsync(selecteduser);
                    if (result.Succeeded)
                    {
                        TempData.Put("message", new AlertMessage()
                        {
                            Title = "User Edited",
                            Message = $"{model.UserName}",
                            AlertType = "success"
                        });
                        return RedirectToAction("UserList");
                    }

                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Error",
                        Message = "Something went wrong",
                        AlertType = "danger"
                    });
                    return RedirectToAction("UserList"); 
                }
                model.SelectedRoles = selectedRoles;
                ViewBag.Roles = roles;
                ModelState.AddModelError("ImageUrl", "Please upload a photo less than 1mb in size");
                return View(model); 
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDelete(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.ImageUrl != "user-logo.png")
            {
                // Deleting old product image
                var pathCurrent = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\user-image", user.ImageUrl);
                FileInfo fileCurrent = new FileInfo(pathCurrent);
                fileCurrent.Delete();
            }

            await _userManager.DeleteAsync(user);

            TempData.Put("message", new AlertMessage()
            {
                Title = "User Deleted",
                Message = $"{user.UserName}",
                AlertType = "warning"
            });
            return RedirectToAction("UserList");
        }

        // ROLE-PART
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles.OrderBy(i => i.Name));
        }

        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var roles = _roleManager.Roles;
            foreach (var r in roles)
            {
                if (r.Name == model.Name.Trim())
                {
                    ModelState.AddModelError("Name", "This role name already exist.");
                    return View(model);
                }
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(System.Web.HttpUtility.HtmlEncode(model.Name.Trim().ToLower())));
            if (result.Succeeded)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Role Created",
                    Message = $"{model.Name}",
                    AlertType = "success"
                });
                return RedirectToAction("RoleList");
            }

            ModelState.AddModelError("Name", "Please enter all values valid");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RoleEdit(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users.ToList())
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
                list.Add(user);
            }

            return View(new RoleEditModel()
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Members = members,
                NonMembers = nonmembers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            if (!string.IsNullOrEmpty(model.RoleName))
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = System.Web.HttpUtility.HtmlEncode(model.RoleName.Trim().ToLower());
                await _roleManager.UpdateAsync(role);
            }

            foreach (var userId in model.IdsToAdd ?? new string[] { })
            {
                var selecteduser = await _userManager.FindByIdAsync(userId);
                if (selecteduser == null)
                {
                    return NotFound();
                }

                var result = await _userManager.AddToRoleAsync(selecteduser, model.RoleName);
                if (!result.Succeeded)
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Error",
                        Message = "Something went wrong.",
                        AlertType = "danger"
                    });
                    return View(model);
                }
            }

            foreach (var userId in model.IdsToDelete ?? new string[] { })
            {
                var selecteduser = await _userManager.FindByIdAsync(userId);
                if (selecteduser == null)
                {
                    return NotFound();
                }

                var result = await _userManager.RemoveFromRoleAsync(selecteduser, model.RoleName);
                if (!result.Succeeded)
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Error",
                        Message = "Something went wrong.",
                        AlertType = "danger"
                    });
                    return View(model);
                }
            }
            
            TempData.Put("message", new AlertMessage()
            {
                Title = "Role Edited",
                Message = $"{model.RoleName}",
                AlertType = "success"
            });
            return Redirect(model.RoleId);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleDelete(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return NotFound();
            }

            await _roleManager.DeleteAsync(role);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Role Deleted",
                Message = $"{role.Name}",
                AlertType = "warning"
            });
            return RedirectToAction("RoleList");           
        }
    
        // CONTACT-PART
        [HttpGet]
        public async Task<IActionResult> ContactListAdmin(ContactAdmin model)
        {
            var messages = await _contactmessageService.GetAll();

            return View(new ContactAdmin() 
            {
                ContactMessages = messages
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactDelete(int MessageId)
        {
            var message = await _contactmessageService.GetById(MessageId);
            _contactmessageService.Delete(message);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Contact Message Deleted",
                Message = $"{message.Message}",
                AlertType = "warning"
            });
            return RedirectToAction("ContactListAdmin");
        }
    
        [HttpGet]
        public async Task<IActionResult> ContactDetailAdmin(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ContactDetailModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }
    }
}