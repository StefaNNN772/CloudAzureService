using DatabaseRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StackOverflowService.Controllers
{
    public class QuestionController : Controller
    {
        // GET: Question

        QuestionRepository repo = new QuestionRepository();
        public ActionResult Index()
        {
            return View();
        }
    }
}