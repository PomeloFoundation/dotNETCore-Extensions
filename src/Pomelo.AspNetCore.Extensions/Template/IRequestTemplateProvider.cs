using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public interface IRequestTemplateProvider
    {
        string DetermineRequestTemplate();
    }
}
