using System.Linq;
using Microsoft.AspNetCore.Http;
using Pomelo.AspNetCore.Extensions.BlobStorage;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public class SignedUserUploadAuthorizationProvider : IBlobUploadAuthorizationProvider
    {
        protected HttpContext httpContext { get; set; }

        public SignedUserUploadAuthorizationProvider (IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
        }

        public bool IsAbleToUpload()
        {
            return httpContext.User.Identities.Count() > 0;
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SignedUserUploadAuthorizationProviderServiceCollectionExtensions
    {
        public static IBlobStorageBuilder AddSignedUserUploadAuthorization(this IBlobStorageBuilder self)
        {
            self.Services.AddSingleton<IBlobUploadAuthorizationProvider, SignedUserUploadAuthorizationProvider>();
            return self;
        }
    }
}