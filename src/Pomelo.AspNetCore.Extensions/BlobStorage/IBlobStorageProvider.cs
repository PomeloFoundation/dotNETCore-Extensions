using System;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IBlobStorageProvider
    {
        Models.Blob Get(Guid id);
        void Delete(Guid id);
        Guid Set(Models.Blob blob);
    }
}
