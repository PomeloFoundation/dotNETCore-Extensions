using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LocalizationSite.Models
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions opt)
            : base(opt)
        {
        }

        public DbSet<Test> Tests { get; set; }
    }
}
