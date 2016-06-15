using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JsonAntiXssSite.Models
{
    public class JsonAntiXssContext : IdentityDbContext
    {
        public JsonAntiXssContext(DbContextOptions opt) : base(opt) { }
    }
}
