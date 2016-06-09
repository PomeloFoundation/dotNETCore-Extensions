using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace Microsoft.EntityFrameworkCore
{
    public interface IBlobStorageDbContext
    {
        DbSet<Blob> Blobs { get; set; }
        int SaveChanges();
    }
}
