using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.Extensions.BlobStorage.Models
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetupBlobStorage<TModel>(this ModelBuilder self)
            where TModel : Blob
        {
            return self.Entity<TModel>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.FileName);
            });
        }

        public static ModelBuilder SetupBlobStorage(this ModelBuilder self)
        {
            return self.SetupBlobStorage<Blob>();
        }
    }
}
