using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Xml;
using Pomelo.Data.Excel.Infrastructure;

namespace Pomelo.Data.Excel
{
    public class ExcelStreamReader : ExcelStreamBase<ExcelStreamReader>, IExcelStreamReader, IDisposable
    {
        public ExcelStreamReader()
        {

        }

        private SheetReader LoadSheetReader(string name, bool hdr)
        {
            var id = WorkBook
                .Where(x => x.Name == name)
                .Select(x => x.Id)
                .First();
            return LoadSheetReader(id, hdr);
        }

        private SheetReader LoadSheetReader(ulong id, bool hdr)
        {
            var worksheet = WorkBook.First(x => x.Id == id);
            var e = ZipArchive.GetEntry($"xl/{worksheet.Target}");

            var sheetStream = e.Open();
            return new SheetReader(sheetStream, CachedSharedStrings, hdr);
        }

        public SheetReader LoadSheetReader(string name)
        {
            return LoadSheetReader(name, false);
        }

        public SheetReader LoadSheetReader(ulong id)
        {
            return LoadSheetReader(id, false);
        }

        public SheetReader LoadSheetReaderHDR(ulong id)
        {
            return LoadSheetReader(id, true);
        }

        public SheetReader LoadSheetReaderHDR(string name)
        {
            return LoadSheetReader(name, true);
        }
    }
}
