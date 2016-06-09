using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pomelo.AspNetCore.Extensions.Localization
{
    public interface IRequestCultureProvider
    {
        HttpContext HttpContext { get; }
        string[] DetermineRequestCulture();
    }
}
