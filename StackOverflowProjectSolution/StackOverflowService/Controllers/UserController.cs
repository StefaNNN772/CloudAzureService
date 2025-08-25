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

                SessionHelper.SetObjectAsJson(Session, "CurrentUser", new UserSession { Email = model.Email});
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
                        // kreiranje blob sadrzaja i kreiranje blob klijenta
                        string uniqueBlobName = string.Format("image_{0}", rowKey);
                        var storageAccount =
                        CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                        CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobStorage.GetContainerReference("vezba");
                        CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                        blob.Properties.ContentType = model.ProfileImage.ContentType;
                        // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                        blob.UploadFromStream(model.ProfileImage.InputStream);
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
                            PictureUrl = blob.Uri.ToString()
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