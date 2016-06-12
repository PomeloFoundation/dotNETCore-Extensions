using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.AspNetCore.TimedJob;

namespace TimeJobSite.Jobs
{
    public class GarbageCollectJob : Job
    {
        [Invoke(Interval = 1000 * 60)]
        public void Collect()
        {
            Console.WriteLine("Collecting...");
            GC.Collect();
        }
    }
}
