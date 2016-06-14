using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.AntiXSS.EntityFramework
{
    public static class AntiXSSModelBuilderExtensions
    {
        public static ModelBuilder SetupAntiXSS(this ModelBuilder self)
        {
            return self.Entity<WhiteListTag>(e =>
            {
                e.HasKey(x => x.Id);
            });
        }
    }
}
