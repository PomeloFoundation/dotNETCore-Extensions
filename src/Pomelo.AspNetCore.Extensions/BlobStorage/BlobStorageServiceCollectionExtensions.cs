using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.AspNetCore.Extensions.BlobStorage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlobServiceCollectionExtensions
    {
        public static IBlobStorageBuilder AddBlobStorage(this IServiceCollection self)
        {
            self.AddContextAccessor();
            var builder = new BlobStorageBuilder();
            builder.Services = self.AddRouting();
            return builder;
        }
    }
}
