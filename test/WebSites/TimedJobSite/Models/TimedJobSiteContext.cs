using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.TimedJob.EntityFramework;

namespace TimedJobSite.Models
{
    public class TimedJobSiteContext : DbContext, ITimedJobContext
    {
        public TimedJobSiteContext(DbContextOptions opt) 
            : base(opt)
        {
        }

        public DbSet<TimedJob> TimedJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.SetupTimedJobs();
        }
    }
}
