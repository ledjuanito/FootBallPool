using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DynamoDBConection;
using FootBallPool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FootBallPool.Controllers
{
    public class AccountController : Controller
    {
        DynamoDBContext context = new DynamoDBContext(new DynamoDBInitializer().client);
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToLocal("");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            User user = context.Load<User>(model.Username);

            //List<ScanCondition> conditions = new List<ScanCondition>();
            //conditions.Add(new ScanCondition("UserID", ScanOperator.Equal, model.Username));
            //conditions.Add(new ScanCondition("Password", ScanOperator.Equal, model.Password));
            //User user = context.Scan<User>(conditions.ToArray()).FirstOrDefault();

            if (user != null && user.Password == model.Password)
            {
                FormsAuthentication.SetAuthCookie(user.UserID, false);
                Session["User"] = user.Info;
                var authTicket = new FormsAuthenticationTicket(1, user.UserID, DateTime.Now, DateTime.Now.AddMinutes(20), false, user.Info.FullName);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);
                return RedirectToLocal(returnUrl);
            }

            else
            {
                ModelState.AddModelError("", "Wrong user or password");
                return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: /User/ChangePassword
        [HttpGet, Authorize]
        public ActionResult ChangePassword(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = context.Load<User>(id); 
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = user.Name + " " + user.LastName + ": " + user.UserID;
            return View();
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string id, ChangePass changepass)
        {
            var done = "Password Updated!";
            var user = (FootBallPool.Models.User.UserInfo)Session["User"];

            if (ModelState.IsValid)
            {
                if (user.user.UserID == id)
                {
                    if (user.user.Password == changepass.OldPassword)
                    {
                        ChangePassDB(id, changepass.NewPassword);
                        ViewBag.JavaScriptFunction = string.Format("ShowErrorPass('{0}');", done);
                    }
                    else
                    {
                        var error = "Worng old password";
                        ViewBag.JavaScriptFunction = string.Format("ShowErrorPass('{0}');", error);
                        ModelState.AddModelError("OldPassword", error);
                    }
                }
            }
            return View();
        }

        public void ChangePassDB(string id, string password)
        {
            User user = context.Load<User>(id);
            if (user != null)
            {
                user.Password = password;
                context.SaveAsync<User>(user);
            }
            
            ////To see entity errors deaitl//****
            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbEntityValidationException ex)
            //{
            //    // Retrieve the error messages as a list of strings.
            //    var errorMessages = ex.EntityValidationErrors
            //            .SelectMany(x => x.ValidationErrors)
            //            .Select(x => x.ErrorMessage);

            //    // Join the list to a single string.
            //    var fullErrorMessage = string.Join("; ", errorMessages);

            //    // Combine the original exception message with the new one.
            //    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

            //    // Throw a new DbEntityValidationException with the improved exception message.
            //    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);               
            //}     
            //*****////
        }

        public void ComparePass(int? roleid, string id, string password, string oldpassword, string newpassword)
        {
            if (password == oldpassword)
            {
                ChangePassDB(id, newpassword);
            }
            else
            {
                var error = "Contraseña Antigua Invalida";
                ViewBag.JavaScriptFunction = string.Format("ShowErrorPass('{0}');", error);
                ViewBag.JavaScriptFunction = string.Format("Redirect('{0}');", roleid);
                ModelState.AddModelError("OldPassword", error);
            }
        }


    }
}