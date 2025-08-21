using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository.Models
{
    public class AlertEmails : TableEntity
    {
        public string Email { get; set; }

        public AlertEmails()
        {
            
        }

        public AlertEmails(string index)
        {
            PartitionKey = "AlertEmails";
            RowKey = index;
            
        }
    }
}
