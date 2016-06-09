using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EFFileUploadProviderServiceCollectionExtensions
    {
        public static IBlobStorageBuilder AddEntityFrameworkStorage<TContext>(this IBlobStorageBuilder self)
            where TContext : DbContext, IBlobStorageDbContext
        {
            self.Services.AddScoped<IBlobStorageProvider, EFFileUploadProvider<TContext>>();
            return self;
        }
    }
}

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public class EFFileUploadProvider<TContext> : IBlobStorageProvider
        where TContext : DbContext, IBlobStorageDbContext
    {
        protected IBlobStorageDbContext DbContext { get; set; }

        public EFFileUploadProvider(TContext db)
        {
            DbContext = db;
        }

        public void Delete(Guid id)
        {
            var blob = DbContext.Blobs
                .SingleOrDefault(x => x.Id == id);
            if (blob != null)
            {
                DbContext.Blobs.Remove(blob);
                DbContext.SaveChanges();
            }
        }

        public Models.Blob Get(Guid id)
        {
            return DbContext.Blobs.Where(x => x.Id == id).SingleOrDefault(); 
        }

        public Guid Set(Models.Blob file)
        {
            if (file.Id != default(Guid) && DbContext.Blobs.Where(x => x.Id == file.Id).SingleOrDefault() != null)
                Delete(file.Id);
            DbContext.Blobs.Add(file);
            DbContext.SaveChanges();
            return file.Id;
        }
    }
}
