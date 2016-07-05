using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        public static int SaveChanges(this DbContext self)
        {
            return self.SaveChanges();
        }
    }
}
