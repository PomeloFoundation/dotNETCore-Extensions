using System;
using Pomelo.AspNetCore.TimedJob;

namespace TimedJobSite.Jobs
{
    public class PrintJob : Job
    {
        public void Print()
        {
            Console.WriteLine("Test dynamic invoke...");
        }
    }
}
