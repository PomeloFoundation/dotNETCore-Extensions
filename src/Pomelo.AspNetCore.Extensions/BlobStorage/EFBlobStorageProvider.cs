using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EFFileUploadProviderServiceCollectionExtensions
    {
        public static IBlobStorageBuilder AddEntityFrameworkStorage<TContext, TModel, TKey>(this IBlobStorageBuilder self)
            where TKey : IEquatable<TKey>
            where TContext : DbContext, IBlobStorageDbContext<TModel, TKey>
            where TModel : Pomelo.AspNetCore.Extensions.BlobStorage.Models.Blob<TKey>
        {
            self.Services.AddScoped<IBlobStorageProvider<TModel, TKey>, EFBlobStorage<TContext, TModel, TKey>>();
            return self;
        }
        public static IBlobStorageBuilder AddEntityFrameworkStorage<TContext>(this IBlobStorageBuilder self)
            where TContext : DbContext, IBlobStorageDbContext<Pomelo.AspNetCore.Extensions.BlobStorage.Models.Blob, Guid>
        {
            return self.AddEntityFrameworkStorage<TContext, Pomelo.AspNetCore.Extensions.BlobStorage.Models.Blob, Guid>();
        }
    }
}

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public class EFBlobStorage<TContext, TModel, TKey> : IBlobStorageProvider<TModel, TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext, IBlobStorageDbContext<TModel, TKey>
        where TModel : Models.Blob<TKey>
    {
        protected IBlobStorageDbContext<TModel, TKey> DbContext { get; set; }

        public EFBlobStorage(TContext db)
        {
            DbContext = db;
        }

        public void Delete(TKey id)
        {
            var blob = DbContext.Blobs
                .SingleOrDefault(x => id.Equals(x.Id));
            if (blob != null)
            {
                DbContext.Blobs.Remove(blob);
                DbContext.SaveChanges();
            }
        }

        public TModel Get(TKey id)
        {
            return DbContext.Blobs.Where(x => id.Equals(x.Id)).SingleOrDefault(); 
        }

        public TKey Set(TModel file)
        {
            if (!file.Id.Equals(default(TKey)) && DbContext.Blobs.Where(x => file.Id.Equals(x.Id)).SingleOrDefault() != null)
                Delete(file.Id);
            DbContext.Blobs.Add(file);
            DbContext.SaveChanges();
            return file.Id;
        }
    }
    public class EFBlobStorage<TContext> : EFBlobStorage<TContext, Models.Blob, Guid>
        where TContext : DbContext, IBlobStorageDbContext<Models.Blob, Guid>
    {
        public EFBlobStorage(TContext db)
            : base(db)
        {
        }
    }
}
