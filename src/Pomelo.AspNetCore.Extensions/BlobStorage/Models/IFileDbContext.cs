using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace Microsoft.EntityFrameworkCore
{
    public interface IBlobStorageDbContext<TModel>
        where TModel : Blob
    {
        DbSet<TModel> Blobs { get; set; }
        int SaveChanges();
    }

    public interface IBlobStorageDbContext : IBlobStorageDbContext<Blob>
    {
    }
}
