using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository.Models
{
    public class NotificationsLog : TableEntity
    {
        public string AnswerId {  get; set; }
        public int EmailsSent { get; set; }
        public DateTime SentAt { get; set; }

        public NotificationsLog() { }

        public NotificationsLog(string index)
        {
            PartitionKey = "NotificationsLog";
            RowKey = index;
        }
    }
}
