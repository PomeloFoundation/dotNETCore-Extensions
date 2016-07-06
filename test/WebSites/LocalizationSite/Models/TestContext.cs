using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LocalizationSite.Models
{
    public class TestContext : LocalizedDbContext
    {
        public TestContext(IServiceProvider services, DbContextOptions opt)
            : base(services, opt)
        {
        }

        public DbSet<Test> Tests { get; set; }
    }
}
