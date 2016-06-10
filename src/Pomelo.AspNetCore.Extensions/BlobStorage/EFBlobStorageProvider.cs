using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EFFileUploadProviderServiceCollectionExtensions
    {
        public static IBlobStorageBuilder AddEntityFrameworkStorage<TContext, TModel>(this IBlobStorageBuilder self)
            where TContext : DbContext, IBlobStorageDbContext<TModel>
            where TModel : Pomelo.AspNetCore.Extensions.BlobStorage.Models.Blob
        {
            self.Services.AddScoped<IBlobStorageProvider<TModel>, EFBlobStorage<TContext, TModel>>();
            return self;
        }
        public static IBlobStorageBuilder AddEntityFrameworkStorage<TContext>(this IBlobStorageBuilder self)
            where TContext : DbContext, IBlobStorageDbContext<Pomelo.AspNetCore.Extensions.BlobStorage.Models.Blob>
        {
            return self.AddEntityFrameworkStorage<TContext, Pomelo.AspNetCore.Extensions.BlobStorage.Models.Blob>();
        }

    }
}

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public class EFBlobStorage<TContext, TModel> : IBlobStorageProvider<TModel>
        where TContext : DbContext, IBlobStorageDbContext<TModel>
        where TModel : Models.Blob
    {
        protected IBlobStorageDbContext<TModel> DbContext { get; set; }

        public EFBlobStorage(TContext db)
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

        public TModel Get(Guid id)
        {
            return DbContext.Blobs.Where(x => x.Id == id).SingleOrDefault(); 
        }

        public Guid Set(TModel file)
        {
            if (file.Id != default(Guid) && DbContext.Blobs.Where(x => x.Id == file.Id).SingleOrDefault() != null)
                Delete(file.Id);
            DbContext.Blobs.Add(file);
            DbContext.SaveChanges();
            return file.Id;
        }
    }
    public class EFBlobStorage<TContext> : EFBlobStorage<TContext, Models.Blob>
        where TContext : DbContext, IBlobStorageDbContext<Models.Blob>
    {
        public EFBlobStorage(TContext db)
            : base(db)
        {
        }
    }
}
