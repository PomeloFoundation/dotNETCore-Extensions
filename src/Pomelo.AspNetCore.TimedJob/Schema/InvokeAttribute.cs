using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    [Invoke(Interval = 1000 * 60 * 60 * 24, SkipWhileExecuting = true)]
    public class InvokeAttribute : Attribute
    {
        public int Interval { get; set; } = 1000 * 60 * 60 * 24; // 24 hours

        public bool SkipWhileExecuting { get; set; } = false;

        public DateTime Begin { get; set; } = DateTime.Now;
    }
}
