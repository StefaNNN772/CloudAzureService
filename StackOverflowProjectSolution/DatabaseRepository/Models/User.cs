using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseRepository
{
    public class User : TableEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PictureUrl { get; set; }

        public User()
        {
            
        }

        public User(string index)
        {
            PartitionKey = "User";
            RowKey = index;
        }
    }
}
