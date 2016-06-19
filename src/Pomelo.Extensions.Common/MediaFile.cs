using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public abstract class MediaFile
    {
        public MediaFile()
        {
            ID = Guid.NewGuid();
        }

        protected string _Source { get; set; }

        public bool IsTemp { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Source { get { return _Source; } }

        public Guid ID { get; set; }
        public virtual byte[] AllBytes { get; set; }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="Path"></param>
        public void SaveAs(string Path)
        {
            System.IO.File.WriteAllBytes(Path, AllBytes);
        }

        public Task SaveAsAsync(string Path)
        {
            return Task.Factory.StartNew(() => SaveAs(Path));
        }

        public void MoveTo(string Path)
        {
            System.IO.File.WriteAllBytes(Path, AllBytes);
            System.IO.File.Delete(_Source);
            _Source = Path;
        }

        public Task MoveToAsync(string Path)
        {
            return Task.Factory.StartNew(() => MoveTo(Path));
        }
    }
}
