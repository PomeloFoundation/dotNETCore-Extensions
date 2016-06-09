using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IUploadAuthorizationProvider
    {
        bool IsAbleToUpload();
    }
}
