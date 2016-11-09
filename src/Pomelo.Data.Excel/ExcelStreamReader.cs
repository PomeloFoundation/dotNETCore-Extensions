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

        public SheetReader LoadSheetReader(string name)
        {
            var id = WorkBook
                .Where(x => x.Name == name)
                .Select(x => x.Id)
                .First();
            return LoadSheetReader(id);
        }

        public SheetReader LoadSheetReader(ulong id)
        {
            var worksheet = WorkBook.First(x => x.Id == id);
            var e = ZipArchive.GetEntry($"xl/{worksheet.Target}");

            var sheetStream = e.Open();
            return new SheetReader(sheetStream, CachedSharedStrings);
        }
    }
}
