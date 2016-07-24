using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IBlobAccessAuthorizationProvider<TKey>
        where TKey : IEquatable<TKey>
    {
        bool IsAbleToDownload(TKey BlobId);
    }
}
