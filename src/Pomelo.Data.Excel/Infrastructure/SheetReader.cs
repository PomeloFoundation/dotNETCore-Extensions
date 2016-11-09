using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using Pomelo.Data.Excel;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class SheetReader : IDisposable
    {
        public SheetReader(Stream sheetStream)
        {
            _sheetStream = sheetStream;
        }

        private readonly Stream _sheetStream;

        public Row ReadNextRow()
        {
            var xmlReader = XmlReader.Create(_sheetStream);

            if (xmlReader.IsStartElement())
            {
                
            }

            return new Row();
        }

        public void Dispose()
        {
            _sheetStream?.Dispose();

            GC.Collect();
        }
    }
}
