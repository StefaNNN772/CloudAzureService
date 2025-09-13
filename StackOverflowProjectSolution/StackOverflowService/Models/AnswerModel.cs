using DatabaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService.Models
{
    public class AnswerModel
    {
        public Answer Answer { get; set; }
        public DatabaseRepository.User User { get; set; }
        public int Score { get; set; }
        public int UserVote { get; set; }
        public bool IsBestAnswer { get; set; }
    }
}