using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using insta_scrape.Classes;
using insta_scrape.Authentication;

namespace insta_scrape.Controllers
{
    public class HomeController : Controller
    {
        private DBController DB = new DBController();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";            
            return View();
        }

        [Authorize]
        public void Logout()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        [Authorize]
        public ActionResult User_Profile()
        {
            ViewBag.Message = "User Profile";
            var userSearches = DB.GetList<UserSearches>(x => x.User.Equals(HttpContext.User.Identity.Name)).ToList();
            ViewBag.MyLinks = userSearches;
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult User_Profile(string new_password, string confirm_password)
        {
            ViewBag.Message = "User Profile";
            var userSearches = DB.GetList<UserSearches>(x => x.User.Equals(HttpContext.User.Identity.Name)).ToList();
            ViewBag.MyLinks = userSearches;
            if(new_password == confirm_password)
            {
                var user = DB.Get<Logins>(x => x.username.Equals(HttpContext.User.Identity.Name));
                user.password = PasswordHelper.GetHash(new_password);
                DB.Update(user);
            }
            return View();
        }


        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Logins model)
        {
            try
            {
                var user = DB.Get<Logins>(x => x.username.Equals(model.username));
                if (user != null)
                {
                    if (user.banned == true)
                    {
                        ViewBag.Error = "User blocked";
                        return View();
                    }
                    if (user.password.Equals(PasswordHelper.GetHash(model.password)))
                    {
                        FormsAuthentication.RedirectFromLoginPage(model.username, true);
                    }
                    else
                    {
                        ViewBag.Error = "Incorrect password";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "User not found";
                    return View();
                }
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message + "\n" + ex.InnerException;
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult Register(){

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Register model)
        {
            try
            {
                var user = DB.Get<Logins>(x => x.username.Equals(model.username));
                if (user == null)
                {
                    if (model.password == model.confirm_password)
                    {
                        var login = new Logins { username = model.username, password = PasswordHelper.GetHash(model.password) };
                        DB.Save(login);
                        ViewBag.Success = "Registration succesful!";
                        return View("Login");
                    }
                    else
                    {
                        ViewBag.Error = "Passwords do not match!";
                        return View();
                    }

                }
                else
                {
                    ViewBag.Error = "Username already taken!";
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message + "\n" + ex.InnerException;
                return View();
            }
            
        }

    }
}