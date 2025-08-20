using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository
{
    public class Answer : TableEntity
    {
        public string UserId { get; set; }
        public string QuestionId { get; set; }
        public string Body { get; set; }

        public Answer()
        {
            
        }

        public Answer(string index)
        {
            PartitionKey = "Answer";
            RowKey = index;
        }
    }
}
