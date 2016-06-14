using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.AntiXSS;
using Pomelo.AspNetCore.AntiXSS.EntityFramework;

namespace Pomelo.AspNetCore.AntiXSS
{
    public class EntityFrameworkWhiteListProvider<TContext> : IWhiteListProvider
        where TContext : IAntiXSSDbContext
    {
        private TContext DB { get; set; }

        private Dictionary<string, string[]> whiteList { get; set; }

        public EntityFrameworkWhiteListProvider(TContext db)
        {
            DB = db;
        }

        public IDictionary<string, string[]> WhiteList
        {
            get
            {
                if (whiteList == null)
                    whiteList = DB.WhiteListTags
                        .Include(x => x.Attributes)
                        .ToList()
                        .ToDictionary(x => x.Id, x => x.Attributes
                            .ToList()
                            .Select(y => y.Attribute)
                            .ToArray());
                return whiteList;
            }
        }

        public void Refresh()
        {
            whiteList = null;
        }
    }
}
