using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.AntiXSS.EntityFramework;

namespace EFAntiXssSite.Models
{
    public class EFAntiXssContext : IdentityDbContext, IAntiXSSDbContext
    {
        public EFAntiXssContext(DbContextOptions opt) : base(opt) { }

        public DbSet<WhiteListAttribute> WhiteListAttributes { get; set; }

        public DbSet<WhiteListTag> WhiteListTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SetupAntiXSS();
        }
    }
}
