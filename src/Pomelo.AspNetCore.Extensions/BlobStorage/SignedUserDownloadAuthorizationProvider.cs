using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public class SignedUserDownloadAuthorizationProvider : IBlobAccessAuthorizationProvider
    {
        protected HttpContext httpContext { get; set; }

        public SignedUserDownloadAuthorizationProvider(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
        }

        public bool IsAbleToDownload(Guid BlobId)
        {
            return httpContext.User.Identities.Count() > 0;
        }
    }
}
