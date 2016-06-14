using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.AntiXSS.EntityFramework
{
    public interface IAntiXSSDbContext
    {
        DbSet<WhiteListTag> WhiteListTags { get; set; }
        DbSet<WhiteListAttribute> WhiteListAttributes { get; set; }

        int SaveChanges();
    }
}
