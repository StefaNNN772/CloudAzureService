using DatabaseRepository.Repositories;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using DatabaseRepository;
using System.Web.Helpers;
using User = DatabaseRepository.User;
using StackOverflowService.Helpers;
using StackOverflowService.Models;

namespace StackOverflowService.Controllers
{
    public class UserController : Controller
    {
        UserRepository repo = new UserRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.User model, string returnUrl)
        {
            //string password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            if (repo.UserExistsLogin(model.Email, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Email, false);

                SessionHelper.SetObjectAsJson(Session, "CurrentUser", new UserSession { Email = model.Email, LoginTime = DateTime.UtcNow});
                //Session["CurrentUser"] = model.Email;
                //Session["LoginTime"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password. Try again.");
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Models.User model)
        {
            if (ModelState.IsValid)
            {
                if (repo.UserExists(model.Email))
                {
                    ModelState.AddModelError("", "User with that email already exist. Try again.");
                    return View(model);
                }
                else
                {
                    try
                    {
                        string password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        string rowKey = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                        var blobUri = BlobHelper.CreateBlobImage(rowKey, model.ProfileImage);

                        if (blobUri == null)
                        {
                            return View(model);
                        }

                        // upis studenta u table storage koristeci StudentDataRepository klasu
                        User user = new User(rowKey)
                        {
                            Name = model.Name,
                            LastName = model.LastName,
                            Gender = model.Gender.ToString(),
                            Country = model.Country,
                            City = model.City,
                            Address = model.Address,
                            Email = model.Email,
                            Password = password,
                            PictureUrl = blobUri
                        };
                        repo.AddUser(user);
                        return RedirectToAction("Login");
                    }
                    catch
                    {
                        return View(model);
                    }
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Edit()
        {
            var userEmail = SessionHelper.GetObjectFromJson<UserSession>(Session, "CurrentUser")?.Email ?? User.Identity.Name;
            var user = repo.GetUser(userEmail);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            Gender gender = user.Gender.ToString().Equals("Male") ? Gender.Male : Gender.Female;

            var model = new Models.User
            {
                Id = user.RowKey,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Country = user.Country,
                City = user.City,
                Address = user.Address,
                Gender = gender,
                PictureUrl = user.PictureUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Models.User model)
        {
            try
            {
                var user = repo.GetUserByRowKey(model.Id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                user.Name = model.Name;
                user.LastName = model.LastName;

                if (!user.Email.Equals(model.Email) && !repo.UserExists(model.Email))
                {
                    user.Email = model.Email;
                }

                user.Country = model.Country;
                user.City = model.City;
                user.Address = model.Address;
                user.Gender = model.Gender.ToString();

                if ((model.ProfileImage != null) && (model.ProfileImage.ContentLength > 0))
                {
                    var blobUri = BlobHelper.CreateBlobImage(user.RowKey, model.ProfileImage);

                    if (blobUri == null)
                    {
                        ModelState.AddModelError("", "An error occurred while updating your profile picture");
                        return View(model);
                    }

                    var blobDeleteResult = BlobHelper.DeleteBlobImage(user.PictureUrl);

                    user.PictureUrl = blobUri;
                }

                if (!string.IsNullOrEmpty(model.Password))
                {
                    string password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                    if (!user.Password.Equals(password))
                    {
                        user.Password = password;
                    }
                }

                repo.UpdateUser(user);

                FormsAuthentication.SignOut();

                Session.Remove("CurrentUser");

                TempData["SuccessMessage"] = "Your profile has been updated successfully!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating your profile: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session.Remove("CurrentUser");

            return RedirectToAction("Index", "Home");
        }
    }
}