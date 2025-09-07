using DatabaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService.Models
{
    public class QuestionModel
    {
        public Question Question { get; set; }
        public DatabaseRepository.User User { get; set; }
        public int AnswersCount { get; set; }
    }
}