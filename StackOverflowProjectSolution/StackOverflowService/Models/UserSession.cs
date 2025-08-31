using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService.Models
{
    [Serializable]
    public class UserSession
    {
        public string Email { get; set; }
        public DateTime? LoginTime { get; set; }
    }
}