using DatabaseRepository.Models;
using DatabaseRepository;
using DatabaseRepository.Repositories;
using StackOverflowService.Helpers;
using StackOverflowService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace StackOverflowService.Controllers
{
    

    public class QuestionsController : Controller
    {
        private UserRepository userRepo = new UserRepository();
        private QuestionRepository questionRepo = new QuestionRepository();
        private AnswerRepository answerRepo = new AnswerRepository();
        private VoteRepository voteRepo = new VoteRepository();
        // GET: Question

        QuestionRepository repo = new QuestionRepository();
        public ActionResult MyQuestions()
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
            var questionsWithDetails = new List<QuestionModel>();

            foreach (var question in questions)
            {
                var user = userRepo.GetUserByRowKey(question.UserId);
                var answersCount = answerRepo.RetrieveAllAnswers()
                    .Where(a => a.QuestionId == question.RowKey).ToList().Count();
                if (question.UserId == currentUserId)
                {
                    questionsWithDetails.Add(new QuestionModel
                    {
                        Question = question,
                        User = user,
                        AnswersCount = answersCount
                    });
                }
            }

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(currentUserId);

            return View(questionsWithDetails);
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
                question = new
                {
                    question.RowKey,
                    question.Title,
                    question.Description,
                    question.ProblemPictureUrl,
                    question.BestAnswerId,
                    FormattedTimestamp = question.Timestamp.ToString("dd.MM.yyyy HH:mm")
                },
                questionUser = questionUser,
                answers = answersWithDetails.Select(a => new {
                    Answer = new
                    {
                        a.Answer.RowKey,
                        a.Answer.Body,
                        FormattedTimestamp = a.Answer.Timestamp.ToString("dd.MM.yyyy HH:mm")
                    },
                    a.User,
                    a.Score,
                    a.UserVote,
                    a.IsBestAnswer
                }),
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

        [HttpPost]
        public JsonResult DeleteQuestion(string rowKey)
        {
            
            try
            {
                QuestionRepository repo = new QuestionRepository();
                
                repo.DeleteQuestion(rowKey);

                return Json(new { success = true, message = "Question deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateQuestion(string rowKey, string title, string description, HttpPostedFileBase problemImage, bool? removeImage)
        {
            try
            {
                // 1. Nađi postojeće pitanje
                var question = questionRepo.GetQuestionByRowKey(rowKey);
                if (question == null)
                {
                    return Json(new { success = false, message = "Question not found." });
                }

                // 2. Izmeni polja
                question.Title = title;
                question.Description = description;

                // 3. Obradi sliku
                if (problemImage != null && problemImage.ContentLength > 0)
                {
                    // Upload nove slike → pregazi staru
                    string imageUrl = UploadImage(problemImage, "question-" + rowKey);
                    question.ProblemPictureUrl = imageUrl;
                }
                else if (removeImage == true)
                {
                    // Ako je označeno uklanjanje slike
                    question.ProblemPictureUrl = null;
                }
                // ako nije poslata nova slika niti remove → ostaje stara

                // 4. Snimi izmenu
                questionRepo.UpdateQuestion(question);

                return Json(new { success = true, message = "Question updated successfully!", data = question });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating question: " + ex.Message });
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

    }
    
}