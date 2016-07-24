using System;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IBlobStorageProvider<TModel, TKey>
        where TKey : IEquatable<TKey>
        where TModel : Models.Blob<TKey>
    {
        TModel Get(TKey id);
        void Delete(TKey id);
        TKey Set(TModel blob);
    }

    public interface IBlobStorageProvider : IBlobStorageProvider<Models.Blob, Guid>
    {
    }
}
