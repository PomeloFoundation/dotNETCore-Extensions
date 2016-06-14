using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pomelo.AspNetCore.Statistics.Middlewares
{
    public class StatisticsMiddleware
    {
        private readonly RequestDelegate next;

        public StatisticsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var url = context.Request.Scheme + "://" + context.Request.Host + "/" + context.Request.Path;

        }
    }
}
