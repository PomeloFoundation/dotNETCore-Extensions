using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AesCryptoExtensions
    {
        public static IServiceCollection AddAesCrypto(this IServiceCollection self, string PrivateKey = null, string IV = null)
        {
            return self.AddSingleton(x => new AesCrypto(PrivateKey, IV));
        }
    }
}
