using System;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IBlobStorageProvider<TModel>
        where TModel : Models.Blob
    {
        TModel Get(Guid id);
        void Delete(Guid id);
        Guid Set(TModel blob);
    }

    public interface IBlobStorageProvider : IBlobStorageProvider<Models.Blob>
    {
    }
}
