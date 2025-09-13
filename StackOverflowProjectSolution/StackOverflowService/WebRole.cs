using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackOverflowService
{
    public class WebRole : RoleEntryPoint
    {
        private HealthManager hmServer = new HealthManager();
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result =  base.OnStart();

            hmServer.Open();
            Console.WriteLine("\n\nHealthManager open!\n\n");
            

            return result;
        }
    }
}
