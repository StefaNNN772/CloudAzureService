using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository.Models
{
    public class Vote : TableEntity
    {
        public string UserId { get; set; }
        public string AnswerId { get; set; }
        public int Value { get; set; }

        public Vote()
        {

        }

        public Vote(string index)
        {
            PartitionKey = "Vote";
            RowKey = index;
        }
    }
}
