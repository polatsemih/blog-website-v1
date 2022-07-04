using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sup1.business.Abstract;
using sup1.entity;
using sup1.webui.EmailServices;
using sup1.webui.Extensions;
using sup1.webui.Identity;
using sup1.webui.Models;
using sup1.webui.Models.Account;
using sup1.webui.Models.Admin;

namespace sup1.webui.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IArticleService _articleService;
        private readonly IContactMessageService _contactmessageService;
        private readonly ICommentService _commentService;
        private readonly ICommentReplyService _commentreplyService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, IArticleService articleService, IContactMessageService contactmessageService, ICommentService commentService, ICommentReplyService commentreplyService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailSender = emailSender;
            this._articleService = articleService;
            this._userManager = userManager;
            this._contactmessageService = contactmessageService;
            this._commentService = commentService;
            this._commentreplyService = commentreplyService;
        }

        // LOGIN
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Password", "Please enter your email and password correctly");
                return View();
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("Email", $"Please confirm your membership with the link in the {model.Email} mailbox");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.Remember, true);
            
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Password", "Your account has been suspended for a short time for security reasons");
                return View();
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("Password", "Your account is inaccessible");
                return View();
            }

            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(user, "admin"))
                    {
                        return RedirectToAction("ArticleAdminList", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Article", "Home");
                    }
                }
            }
            
            ModelState.AddModelError("Password", "Please enter your email and password correctly");
            return View();            
        }
        
        // REGISTER
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UserName.Contains(" "))
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Input",
                    Message = "User name can not contain space.",
                    AlertType = "danger"
                });
                return View(model);
            }
            
            var user = new User()
            {
                FirstName = System.Web.HttpUtility.HtmlEncode(model.FirstName.Trim()),
                LastName = System.Web.HttpUtility.HtmlEncode(model.LastName.Trim()),
                UserName = System.Web.HttpUtility.HtmlEncode(model.UserName.Trim()),
                Email = System.Web.HttpUtility.HtmlEncode(model.Email),
                ImageUrl = "user-logo.png"
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = token
                });

                string htmlMessage ="<!DOCTYPE html>" +
                                "<html lang=\"en\">" +
                                    "<head>" +
                                        "<meta charset=\"UTF-8\">" +
                                        "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
                                        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                                        "<title>SUP Blog Website</title>" +
                                    "</head>" +
                                    "<body style=\"margin: 0; padding: 0; border: 0;\">" +
                                        "<div style=\"min-height: 100%; max-width: 800px; background-color: #FFFFFF; margin-left: auto; margin-right: auto; text-align: center;\">" +
                                            "<header style=\"padding-top: 50px; padding-bottom: 50px; background-image: url('https://images.unsplash.com/photo-1527254432336-72d0c3ad1942?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=1951&q=80'); background-repeat: no-repeat; background-position: center; background-size: cover;\">" +
                                                "<h1 style=\"font-size: 40px; color: #03D29F;\">Confirm Your Account</h1>" +
                                                $"<h2 style=\"font-size: 20px; padding-bottom: 10px; color: #000000;\">Hello {model.FirstName}</h2>" +
                                                "<p style=\"font-size: 20px; padding-bottom: 10px; color: #000000;\">Click the confirm button to verify your SUP Blog Website Account</p>" +
                                                $"<a href=\"http://localhost:5000{url}\" style=\"font-size: 20px; text-decoration: none; background-color: #03D29F; color: #FFFFFF; padding: 8px 12px; border-radius: 5px;\">Confirm</a>" +
                                            "</header>" +
                                            "<footer style=\"background-color: #000000; padding-top: 10px; padding-bottom: 10px;\">" +
                                                "<p style=\"font-size: 14px; color: #FFFFFF; opacity: 0.7;\">&#169; 2021-2022 SUP</p>" +
                                                "<p style=\"font-size: 14px; color: #FFFFFF;\">For your questions and offers, contact <a href=\"mailto:utkupolat@protonmail.com\" style=\"font-size: 14px; color: #a76c00;\">utkupolat@protonmail.com</a></p>" +
                                            "</footer>" +
                                        "</div>" +
                                    "</body>" +
                                "</html>";

                await _emailSender.SendEmailAsync(model.Email, "Confirm SUP Blog Website Account", htmlMessage);

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Verification",
                    Message = "Please verify your account with the link in your mailbox. The incoming e-mail may have gone to the spam box",
                    AlertType = "success"
                });
                return RedirectToAction("Login");
            }

            TempData.Put("message", new AlertMessage()
            {
                Title = "Error",
                Message = "Something went wrong. Please try again to register",
                AlertType = "danger"
            });
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Verification",
                    Message = "Invalid operation",
                    AlertType = "danger"
                });
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Verification",
                    Message = "Your account has been succesfuly created. You can log in now",
                    AlertType = "success"
                });
                return RedirectToAction("Login");
            }

            return RedirectToAction("Login");
        }

        // LOGOUT
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message", new AlertMessage()
            {
                Title = "Logged Out",
                Message = "Your session was securely terminated",
                AlertType = "success"
            });
            return RedirectToAction("Login");
        }

        // ACCESSDENIED
        public IActionResult AccessDenied()
        {
            return View();
        }

        // PASSWORD
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
           
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email could not found.");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = token
            });

            string htmlMessage ="<!DOCTYPE html>" +
                                "<html lang=\"en\">" +
                                    "<head>" +
                                        "<meta charset=\"UTF-8\">" +
                                        "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
                                        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                                        "<title>SUP Blog Website</title>" +
                                    "</head>" +
                                    "<body style=\"margin: 0; padding: 0; border: 0;\">" +
                                        "<div style=\"min-height: 100%; max-width: 800px; background-color: #FFFFFF; margin-left: auto; margin-right: auto; text-align: center;\">" +
                                            "<header style=\"padding-top: 50px; padding-bottom: 50px; background-image: url('https://images.unsplash.com/photo-1527254432336-72d0c3ad1942?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=1951&q=80'); background-repeat: no-repeat; background-position: center; background-size: cover;\">" +
                                                "<h1 style=\"font-size: 40px; color: #03D29F;\">Reset Your Password</h1>" +
                                                $"<h2 style=\"font-size: 20px; padding-bottom: 10px; color: #000000;\">Hello {user.FirstName}</h2>" +
                                                "<p style=\"font-size: 20px; padding-bottom: 10px; color: #000000;\">Click the reset button to reset your SUP Blog Website Account password</p>" +
                                                $"<a href=\"http://localhost:5000{url}\" style=\"font-size: 20px; text-decoration: none; background-color: #03D29F; color: #FFFFFF; padding: 8px 12px; border-radius: 5px;\">Reset Password</a>" +
                                            "</header>" +
                                            "<footer style=\"background-color: #000000; padding-top: 10px; padding-bottom: 10px;\">" +
                                                "<p style=\"font-size: 14px; color: #FFFFFF; opacity: 0.7;\">&#169; 2021-2022 SUP</p>" +
                                                "<p style=\"font-size: 14px; color: #FFFFFF;\">For your questions and offers, contact <a href=\"mailto:utkupolat@protonmail.com\" style=\"font-size: 14px; color: #a76c00;\">utkupolat@protonmail.com</a></p>" +
                                            "</footer>" +
                                        "</div>" +
                                    "</body>" +
                                "</html>";

            await _emailSender.SendEmailAsync(model.Email, "Reset Password SUP Blog Website", htmlMessage);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Reset Password",
                Message = "Click on the link in your mailbox to reset your password. The incoming e-mail may have gone to the spam box",
                AlertType = "success"
            });
            return RedirectToAction("Login", "Account");
            
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);

            return View(new ResetPasswordModel 
            { 
                Token = token,
                Email = user.Email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            if (string.IsNullOrEmpty(model.Token))
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Reset Password",
                    Message = "Password changed successfully",
                    AlertType = "success"
                });
                return RedirectToAction("Login", "Account");
            }
            
            TempData.Put("message", new AlertMessage()
            {
                Title = "Error",
                Message = "The password could not be changed",
                AlertType = "danger"
            });
            return RedirectToAction("Login", "Account");
        }

        // USER-PROFILE
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.FindByIdAsync(User.Claims.ElementAt(0).Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserProfileModel()
            {
                FirstName = User.Claims.ElementAt(4).Value,
                LastName = User.Claims.ElementAt(5).Value,
                UserName = User.Claims.ElementAt(1).Value,
                Email = User.Claims.ElementAt(2).Value,
                ImageUrl = User.Claims.ElementAt(6).Value
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(UserProfileModel model, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = await _userManager.FindByIdAsync(User.Claims.ElementAt(0).Value);
            if (user == null)
            {
                return NotFound();   
            }

            if (model.UserName.Trim() != user.UserName)
            {
                var users = _userManager.Users;
                foreach (var u in users)
                {
                    if (u.UserName == model.UserName.Trim())
                    {
                        ModelState.AddModelError("UserName","This user name is already taken");
                        return View(model);
                    }
                }
            }

            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                if (model.UserName.Trim() != null)
                {
                    var articles = await _articleService.GetAll();
                    foreach (var article in articles)
                    {
                        if (article.Author == user.UserName)
                        {
                            article.Author = model.UserName.Trim();
                            _articleService.Update(article);
                        }
                    }
                }
            }

            if (model.NewPassword != null)
            {
                await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }

            if (file == null)
            {
                user.FirstName = System.Web.HttpUtility.HtmlEncode(model.FirstName.Trim());
                user.LastName = System.Web.HttpUtility.HtmlEncode(model.LastName.Trim());
                user.UserName = System.Web.HttpUtility.HtmlEncode(model.UserName.Trim());
                user.Email = System.Web.HttpUtility.HtmlEncode(model.Email);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Logged Out",
                        Message = "Your informations succesfuly updated. Please log in again",
                        AlertType = "success"
                    });
                    return RedirectToAction("Login");
                }

                await _signInManager.SignOutAsync();
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Logged Out",
                    Message = "Something went wrong",
                    AlertType = "danger"
                });
                return RedirectToAction("Login");            
            }
            else
            {
                if (file.Length < (1024 * 1024 * 1))
                {
                    if (user.ImageUrl != "user-logo.png")
                    {
                        // Deleting old product image
                        var pathCurrent = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\user-image", user.ImageUrl);
                        FileInfo fileCurrent = new FileInfo(pathCurrent);
                        fileCurrent.Delete();
                    }

                    // Insertig new product image
                    var randomnumber = new Random().Next(99, 999999999);
                    var randomName = string.Format($"{randomnumber}{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}");
                    user.ImageUrl = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\user-image", randomName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    user.FirstName = System.Web.HttpUtility.HtmlEncode(model.FirstName.Trim());
                    user.LastName = System.Web.HttpUtility.HtmlEncode(model.LastName.Trim());
                    user.UserName = System.Web.HttpUtility.HtmlEncode(model.UserName.Trim());
                    user.Email = System.Web.HttpUtility.HtmlEncode(model.Email);
                    
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignOutAsync();
                        TempData.Put("message", new AlertMessage()
                        {
                            Title = "Logged Out",
                            Message = "Your informations succesfuly updated. Please log in again",
                            AlertType = "success"
                        });
                        return RedirectToAction("Login");
                    }

                    await _signInManager.SignOutAsync();
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Logged Out",
                        Message = "Something wen wrong",
                        AlertType = "danger"
                    });
                    return RedirectToAction("Login");        
                }
                ModelState.AddModelError("ImageUrl", "Please upload a photo less than 1mb in size");
                return View(model);                    
            }
        }
    
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDelete()
        {
            var user = await _userManager.FindByIdAsync(User.Claims.ElementAt(0).Value);
            if (user == null)
            {
                return NotFound();
            }

            if (User.Claims.ElementAt(6).Value != "user-logo.png")
            {
                // Deleting old product image
                var pathCurrent = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\user-image", User.Claims.ElementAt(6).Value);
                FileInfo fileCurrent = new FileInfo(pathCurrent);
                fileCurrent.Delete();
            }

            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index","Home");
        }

        // CONTACT PART
        [Authorize]
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            if (string.IsNullOrEmpty(model.Message) || string.IsNullOrEmpty(model.Subject)) 
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Input",
                    Message = "Please enter all fields valid",
                    AlertType = "danger"
                });
                return View(model);
            }

            var contactmessage = new ContactMessage()
            {
                UserId = User.Claims.ElementAt(0).Value,
                Subject = System.Web.HttpUtility.HtmlEncode(model.Subject.Trim()),
                Message = System.Web.HttpUtility.HtmlEncode(model.Message.Trim())
            };
            _contactmessageService.Create(contactmessage);
            
            TempData.Put("message", new AlertMessage()
            {
                Title = "Contact Message",
                Message = "Your message has been sent. Thank you for your feed-back",
                AlertType = "success"
            });
            return RedirectToAction("Contact", "Account");
        }
        
        // COMMENT
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentCreate(ArticleCommentModel model)   
        {
            var encodedUrl = WebUtility.UrlEncode(model.Url);

            if (string.IsNullOrEmpty(model.CommentMessage))
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Input",
                    Message = "Please write a comment meessage",
                    AlertType = "danger"
                });
                return Redirect("/article/detail/" + encodedUrl);
            }           

            var comment = new Comment()
            {
                UserId = User.Claims.ElementAt(0).Value,
                UserName = User.Claims.ElementAt(1).Value,
                UserImageUrl = User.Claims.ElementAt(6).Value,
                Message = System.Web.HttpUtility.HtmlEncode(model.CommentMessage.Trim()),
                ArticleId = model.ArticleId
            };
            _commentService.Create(comment);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Comment Created",
                Message = "Thank you for your comment",
                AlertType = "success"
            });
            return Redirect("/article/detail/" + encodedUrl);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentReplyCreate(ArticleCommentModel model)   
        {
            var encodedUrl = WebUtility.UrlEncode(model.Url);

            if (string.IsNullOrEmpty(model.CommentReplyMessage))
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Input",
                    Message = "Please enter the comment reply meessage",
                    AlertType = "danger"
                });
                return Redirect("/article/detail/" + encodedUrl);
            }

            var commentreply = new CommentReply()
            {
                UserId = User.Claims.ElementAt(0).Value,
                UserName = User.Claims.ElementAt(1).Value,
                UserImageUrl = User.Claims.ElementAt(6).Value,
                Message = System.Web.HttpUtility.HtmlEncode(model.CommentReplyMessage.Trim()),
                CommentId = model.CommentId
            };
            _commentreplyService.Create(commentreply);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Comment Reply Created",
                Message = "Thank you for your comment",
                AlertType = "success"
            });
            return Redirect("/article/detail/" + encodedUrl);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentDelete(ArticleCommentModel model)
        {
            var comment = await _commentService.GetById(model.CommentId);
            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != User.Claims.ElementAt(0).Value)
            {
                return NotFound();
            }
            
            _commentService.Delete(comment);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Comment Deleted",
                AlertType = "danger"
            });
            
            var encodedUrl = WebUtility.UrlEncode(model.Url);
            return Redirect("/article/detail/" + encodedUrl);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentReplyDelete(ArticleCommentModel model)
        {   
            var commentreply = await _commentreplyService.GetById(model.CommentReplyId);
            if (commentreply == null)
            {
                return NotFound();
            }

            if (commentreply.UserId != User.Claims.ElementAt(0).Value)
            {
                return NotFound();
            }
            
            _commentreplyService.Delete(commentreply);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Comment Reply Deleted",
                AlertType = "danger"
            });
            
            var encodedUrl = WebUtility.UrlEncode(model.Url);
            return Redirect("/article/detail/" + encodedUrl);
        }
    }
}