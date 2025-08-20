using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository
{
    public class Question : TableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProblemPictureUrl { get; set; }
        public string UserId { get; set; }
        public string BestAnswerId { get; set; } = "";

        public Question()
        {
            
        }

        public Question(string index)
        {
            PartitionKey = "Question";
            RowKey = index;
        }
    }
}
