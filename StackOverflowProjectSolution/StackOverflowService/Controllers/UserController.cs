using DatabaseRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StackOverflowService.Controllers
{
    public class UserController : Controller
    {
        // GET: User

        UserRepository repo = new UserRepository();

        public ActionResult Index()
        {
            return View();
        }
    }
}