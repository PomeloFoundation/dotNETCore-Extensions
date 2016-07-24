using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.Extensions.BlobStorage.Models
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetupBlobStorage<TModel, TKey>(this ModelBuilder self)
            where TKey : IEquatable<TKey>
            where TModel : Blob<TKey>
        {
            return self.Entity<TModel>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.FileName);
            });
        }

        public static ModelBuilder SetupBlobStorage<TKey>(this ModelBuilder self)
            where TKey : IEquatable<TKey>
        {
            return self.SetupBlobStorage<Blob<TKey>, TKey>();
        }

        public static ModelBuilder SetupBlobStorage(this ModelBuilder self)
        {
            return self.SetupBlobStorage<Blob, Guid>();
        }
    }
}
