using System;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AspNetCore.TimedJob;

namespace TimedJobSite.Models
{
    public static class SampleData
    {
        public static void InitDB(IServiceProvider services)
        {
            var DB = services.GetRequiredService<TimedJobSiteContext>();
            var TimedJobService = services.GetRequiredService<TimedJobService>();
            DB.Database.EnsureCreated();
            DB.TimedJobs.Add(new Pomelo.AspNetCore.TimedJob.EntityFramework.TimedJob
            {
                Id = "TimedJobSite.Jobs.GarbageCollectJob.Print",
                Begin = DateTime.Now,
                Interval = 3000,
                IsEnabled = true
            });
            DB.SaveChanges();
            TimedJobService.RestartDynamicTimers();
        }
    }
}
