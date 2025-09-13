using DatabaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService.Models
{
    public class QuestionDetailsModel
    {
        public Question Question { get; set; }
        public DatabaseRepository.User QuestionUser { get; set; }
        public List<AnswerModel> Answers { get; set; }
        public bool CanMarkBest { get; set; }
        public bool IsQuestionClosed { get; set; }
    }
}