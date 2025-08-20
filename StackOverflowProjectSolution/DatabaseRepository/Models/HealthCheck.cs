using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository.Models
{
    public class HealthCheck : TableEntity
    {
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string ServiceName { get; set; }

        public HealthCheck()
        {
            
        }

        public HealthCheck(string index)
        {
            PartitionKey = "HealthCheck";
            RowKey = index;
        }
    }
}
