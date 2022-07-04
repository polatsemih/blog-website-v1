using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using sup1.business.Abstract;
using sup1.business.Concrete;
using sup1.data.Abstract;
using sup1.data.Concrete.EfCore;
using sup1.webui.EmailServices;
using sup1.webui.Factory;
using sup1.webui.Identity;

namespace sup1.webui
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AccountContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MsSqlConnection")));
            services.AddDbContext<SupContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MsSqlConnection")));

            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AccountContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // User settings.
                // options.User.AllowedUserNameCharacters = 
                // "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Lockout
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
                options.Lockout.AllowedForNewUsers = true;

                // Requirements
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(32);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".Sup.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IArticleService, ArticleManager>();
            services.AddScoped<IArticleContentImageService, ArticleContentImageManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IContactMessageService, ContactMessageManager>();
            services.AddScoped<ICommentService, CommentManager>();
            services.AddScoped<ICommentReplyService, CommentReplyManager>();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsFactory>();

            services.AddScoped<IEmailSender, SmtpEmailSender>(i =>
                new SmtpEmailSender
                (
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"]
                )
            );

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "wwwroot")
                ),
                RequestPath = "/folder"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "node_modules")
                ),
                RequestPath = "/library"
            });

            app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "article",
                    pattern: "articles",
                    defaults: new { controller = "Home", action = "Article" }
                );

                endpoints.MapControllerRoute(
                    name: "articlecategory",
                    pattern: "articles/{category?}",
                    defaults: new { controller = "Home", action = "ArticleCategory" }
                );

                endpoints.MapControllerRoute(
                    name: "login",
                    pattern: "account/login",
                    defaults: new { controller = "Account", action = "Login" }
                );

                endpoints.MapControllerRoute(
                    name: "register",
                    pattern: "account/register",
                    defaults: new { controller = "Account", action = "Register" }
                );

                endpoints.MapControllerRoute(
                    name: "forgotpassword",
                    pattern: "account/forgotpassword",
                    defaults: new { controller = "Account", action = "ForgotPassword" }
                );

                endpoints.MapControllerRoute(
                    name: "resetpassword",
                    pattern: "account/resetpassword",
                    defaults: new { controller = "Account", action = "ResetPassword" }
                );

                endpoints.MapControllerRoute(
                    name: "resetpassword",
                    pattern: "account/profile",
                    defaults: new { controller = "Account", action = "UserProfile" }
                );

                endpoints.MapControllerRoute(
                    name: "articleadminlist",
                    pattern: "article/admin",
                    defaults: new { controller = "Admin", action = "ArticleAdminList" }
                );

                endpoints.MapControllerRoute(
                    name: "articleuploadimage",
                    pattern: "article/uploadimage",
                    defaults: new { controller = "Admin", action = "ArticleUploadImage" }
                );

                endpoints.MapControllerRoute(
                    name: "admincontactmessages",
                    pattern: "admin/contact/messages",
                    defaults: new { controller = "Admin", action = "ContactListAdmin" }
                );

                endpoints.MapControllerRoute(
                    name: "admincategories",
                    pattern: "admin/category/list",
                    defaults: new { controller = "Admin", action = "CategoryList" }
                );

                endpoints.MapControllerRoute(
                    name: "admincategorycreate",
                    pattern: "admin/category/create",
                    defaults: new { controller = "Admin", action = "CategoryCreate" }
                );  

                endpoints.MapControllerRoute(
                    name: "adminusercreate",
                    pattern: "admin/user/create",
                    defaults: new { controller = "Admin", action = "UserCreate" }
                );

                endpoints.MapControllerRoute(
                    name: "articlecreate",
                    pattern: "admin/article/create",
                    defaults: new { controller = "Admin", action = "ArticleCreate" }
                );

                endpoints.MapControllerRoute(
                    name: "articlecontentcreate",
                    pattern: "admin/article-content/create",
                    defaults: new { controller = "Admin", action = "ArticleContentCreate" }
                );

                endpoints.MapControllerRoute(
                    name: "articlecontentedit",
                    pattern: "admin/article-content/edit",
                    defaults: new { controller = "Admin", action = "ArticleContentEdit" }
                );

                endpoints.MapControllerRoute(
                    name: "adminrolecreate",
                    pattern: "admin/role/create",
                    defaults: new { controller = "Admin", action = "RoleCreate" }
                );

                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/roles/list",
                    defaults: new { controller = "Admin", action = "RoleList" }
                );

                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/users/list",
                    defaults: new { controller = "Admin", action = "UserList" }
                );

                endpoints.MapControllerRoute(
                    name: "articledetail",
                    pattern: "article/detail/{articleUrl?}",
                    defaults: new { controller = "Home", action = "ArticleDetail" }
                );

                endpoints.MapControllerRoute(
                    name: "admincategoryedit",
                    pattern: "admin/category/{categoryId?}",
                    defaults: new { controller = "Admin", action = "CategoryEdit" }
                );

                endpoints.MapControllerRoute(
                    name: "adminroleedit",
                    pattern: "admin/role/{RoleId?}",
                    defaults: new { controller = "Admin", action = "RoleEdit" }
                );

                endpoints.MapControllerRoute(
                    name: "adminroleedit",
                    pattern: "admin/user/{UserId?}",
                    defaults: new { controller = "Admin", action = "UserEdit" }
                );

                endpoints.MapControllerRoute(
                    name: "contact",
                    pattern: "account/contact/{userName?}",
                    defaults: new { controller = "Account", action = "Contact" }
                ); 

                endpoints.MapControllerRoute(
                    name: "articleedit",
                    pattern: "admin/article/edit/{articleId?}",
                    defaults: new { controller = "Admin", action = "ArticleEdit" }
                );

                endpoints.MapControllerRoute(
                    name: "search",
                    pattern: "article/search",
                    defaults: new { controller = "Home", action = "ArticleSearch" }
                ); 

                endpoints.MapControllerRoute(
                    name: "articlecategoryadmin",
                    pattern: "admin/articles/{category?}",
                    defaults: new { controller = "Admin", action = "ArticleCategoryAdmin" }
                );

                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/users/list/details",
                    defaults: new { controller = "Admin", action = "UserDetail" }
                );      

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}"
                );
            });

            SeedIdentity.Seed(userManager, roleManager, configuration).Wait();
        }
    }
}
