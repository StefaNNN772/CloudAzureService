using DatabaseRepository.Models;
using DatabaseRepository;
using DatabaseRepository.Repositories;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StackOverflowService.Helpers;
using StackOverflowService.Models;

namespace StackOverflowService.Controllers
{
    public class HomeController : Controller
    {
        private UserRepository userRepo = new UserRepository();
        private QuestionRepository questionRepo = new QuestionRepository();
        private AnswerRepository answerRepo = new AnswerRepository();
        private VoteRepository voteRepo = new VoteRepository();
        public ActionResult Index()
        {
            // Dobijanje korisnika iz sesije
            var currentUser = SessionHelper.GetObjectFromJson<UserSession>(Session, "CurrentUser");
            string currentUserId = null;

            if (currentUser != null)
            {
                // Pronađi user ID na osnovu email-a
                var user = userRepo.GetUser(currentUser.Email);
                currentUserId = user?.RowKey;
            }

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(currentUserId);

            // Dobijanje svih pitanja sa dodatnim informacijama
            var questions = questionRepo.RetrieveAllQuestions().ToList();
            var questionsWithDetails = new List<dynamic>();

            foreach (var question in questions)
            {
                var user = userRepo.GetUserByRowKey(question.UserId);
                var answersCount = answerRepo.RetrieveAllAnswers()
                    .Where(a => a.QuestionId == question.RowKey).ToList().Count();


                questionsWithDetails.Add(new
                {
                    Question = question,
                    User = user,
                    AnswersCount = answersCount
                });
            }

            ViewBag.Questions = questionsWithDetails;
            return View();
        }

        //Post metoda za postavljanje pitanja
        [HttpPost]
        public ActionResult CreateQuestion(string title, string description, HttpPostedFileBase problemImage)
        {
            var currentUser = SessionHelper.GetObjectFromJson<UserSession>(Session, "CurrentUser");
            string currentUserId = null;
            if (currentUser != null)
            {
                var user = userRepo.GetUser(currentUser.Email);
                currentUserId = user?.RowKey;
            }
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, message = "You need to be logged in to ask a question" });
            }

            try
            {
                var questionId = Guid.NewGuid().ToString();
                var question = new Question(questionId)
                {
                    Title = title,
                    Description = description,
                    UserId = currentUserId,
                    BestAnswerId = ""
                };

                // Upload slike ako postoji
                if (problemImage != null && problemImage.ContentLength > 0)
                {
                    string imageUrl = UploadImage(problemImage, "question-" + questionId);
                    question.ProblemPictureUrl = imageUrl;
                }

                questionRepo.AddQuestion(question);

                return Json(new { success = true, message = "Question asked successfuly!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error when asking a question: " + ex.Message });
            }
        }

        //Get metoda za pribavljanje detalje o pitanju
        [HttpGet]
        public ActionResult GetQuestionDetails(string questionId)
        {
            var question = questionRepo.RetrieveAllQuestions()
                .Where(q => q.RowKey == questionId).FirstOrDefault();

            if (question == null)
            {
                return Json(new { success = false, message = "Question not found." }, JsonRequestBehavior.AllowGet);
            }

            var questionUser = userRepo.GetUserByRowKey(question.UserId);
            var answers = answerRepo.RetrieveAllAnswers()
                .Where(a => a.QuestionId == questionId).ToList();

            var currentUser = SessionHelper.GetObjectFromJson<UserSession>(Session, "CurrentUser");
            string currentUserId = null;
            if (currentUser != null)
            {
                var user = userRepo.GetUser(currentUser.Email);
                currentUserId = user?.RowKey;
            }
            var answersWithDetails = new List<dynamic>();

            foreach (var answer in answers)
            {
                var answerUser = userRepo.GetUserByRowKey(answer.UserId);
                var votes = voteRepo.RetrieveAllVotes()
                    .Where(v => v.AnswerId == answer.RowKey).ToList();

                var score = votes.Sum(v => v.Value);
                var userVote = votes.Where(v => v.UserId == currentUserId).FirstOrDefault();

                answersWithDetails.Add(new
                {
                    Answer = answer,
                    User = answerUser,
                    Score = score,
                    UserVote = userVote?.Value ?? 0,
                    IsBestAnswer = answer.RowKey == question.BestAnswerId
                });
            }

            // Sortiranje odgovora gdje najbolji odgovor ide na vrh
            answersWithDetails = answersWithDetails
                .OrderByDescending(a => a.IsBestAnswer)
                .ThenByDescending(a => a.Score)
                .ToList();

            return Json(new
            {
                success = true,
                question = question,
                questionUser = questionUser,
                answers = answersWithDetails,
                canMarkBest = currentUserId == question.UserId && string.IsNullOrEmpty(question.BestAnswerId),
                isQuestionClosed = !string.IsNullOrEmpty(question.BestAnswerId)
            }, JsonRequestBehavior.AllowGet);
        }

        //Postavljanje odgovora
        [HttpPost]
        public ActionResult SubmitAnswer(string questionId, string answerBody)
        {
            var currentUser = SessionHelper.GetObjectFromJson<UserSession>(Session, "CurrentUser");
            string currentUserId = null;
            if (currentUser != null)
            {
                var user = userRepo.GetUser(currentUser.Email);
                currentUserId = user?.RowKey;
            }
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, message = "You need to be logged in to answer." });
            }

            // Proveriti da li je pitanje zatvoreno
            var question = questionRepo.RetrieveAllQuestions()
                .Where(q => q.RowKey == questionId).FirstOrDefault();

            if (question != null && !string.IsNullOrEmpty(question.BestAnswerId))
            {
                return Json(new { success = false, message = "Question is finished, best answer already found." });
            }

            try
            {
                var answerId = Guid.NewGuid().ToString();
                var answer = new Answer(answerId)
                {
                    QuestionId = questionId,
                    UserId = currentUserId,
                    Body = answerBody
                };

                answerRepo.AddAnswer(answer);

                return Json(new { success = true, message = "Answer succesfully sent!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while sending anser: " + ex.Message });
            }
        }

        //Metoda za glasanje (downvote/upvote)
        [HttpPost]
        public ActionResult VoteAnswer(string answerId, int voteValue)
        {
            var currentUser = SessionHelper.GetObjectFromJson<UserSession>(Session, "CurrentUser");
            string currentUserId = null;
            if (currentUser != null)
            {
                var user = userRepo.GetUser(currentUser.Email);
                currentUserId = user?.RowKey;
            }
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, message = "You need to be logged in to answer." });
            }

            try
            {
                // Proveriti da li je korisnik već glasao
                //Moze se kasnije promijeniti metoda da korisnik moze da mijenja glasove
                var existingVote = voteRepo.RetrieveAllVotes()
                    .Where(v => v.UserId == currentUserId && v.AnswerId == answerId)
                    .FirstOrDefault();

                if (existingVote != null)
                {
                    return Json(new { success = false, message = "You already voted for this answer" });
                }

                var voteId = Guid.NewGuid().ToString();
                var vote = new Vote(voteId)
                {
                    UserId = currentUserId,
                    AnswerId = answerId,
                    Value = voteValue // 1 za upvote, -1 za downvote
                };

                voteRepo.AddVote(vote);

                // Vratiti novi score
                var allVotes = voteRepo.RetrieveAllVotes()
                    .Where(v => v.AnswerId == answerId).ToList();
                var newScore = allVotes.Sum(v => v.Value);

                return Json(new { success = true, newScore = newScore });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while voting: " + ex.Message });
            }
        }

        //Postavljanje najboljeg odgovora
        [HttpPost]
        public ActionResult MarkBestAnswer(string questionId, string answerId)
        {
            var currentUser = SessionHelper.GetObjectFromJson<UserSession>(Session, "CurrentUser");
            string currentUserId = null;
            if (currentUser != null)
            {
                var user = userRepo.GetUser(currentUser.Email);
                currentUserId = user?.RowKey;
            }
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, message = "You need to be logged in." });
            }

            try
            {
                var question = questionRepo.RetrieveAllQuestions()
                    .Where(q => q.RowKey == questionId).FirstOrDefault();

                if (question == null)
                {
                    return Json(new { success = false, message = "Question not found." });
                }

                if (question.UserId != currentUserId)
                {
                    return Json(new { success = false, message = "Only the question maker can vote." });
                }

                if (!string.IsNullOrEmpty(question.BestAnswerId))
                {
                    return Json(new { success = false, message = "Best answer already decided." });
                }

                question.BestAnswerId = answerId;
                questionRepo.UpdateQuestion(question);

                return Json(new { success = true, message = "Najbolji odgovor je označen!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Greška pri označavanju najboljeg odgovora: " + ex.Message });
            }
        }

        //Postavljanje slika za pitanje
        private string UploadImage(HttpPostedFileBase file, string fileName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("DataConnectionString"));

                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("stackoverflow");
                container.CreateIfNotExists();

                var permissions = container.GetPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);

                string uniqueBlobName = string.Format("{0}_{1}", fileName, DateTime.Now.Ticks);
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = file.ContentType;
                blob.UploadFromStream(file.InputStream);

                return blob.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Greška pri uploadu slike: " + ex.Message);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}