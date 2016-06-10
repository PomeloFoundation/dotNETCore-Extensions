using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IBlobAccessAuthorizationProvider
    {
        bool IsAbleToDownload(Guid BlobId);
    }
}
