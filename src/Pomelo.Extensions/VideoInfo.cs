using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class VideoInfo
    {
        /// <summary>
        /// 片长
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// 比特率
        /// </summary>
        public int Biterate { get; set; }

        /// <summary>
        /// 宽(px)
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高(px)
        /// </summary>
        public int Height { get; set; }
    }
}
