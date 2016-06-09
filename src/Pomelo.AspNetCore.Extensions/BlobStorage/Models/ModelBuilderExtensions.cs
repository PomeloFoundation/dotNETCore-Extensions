using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.Extensions.BlobStorage.Models
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetupFiles(this ModelBuilder self)
        {
            return self.Entity<Blob>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.FileName);
            });
        }
    }
}
