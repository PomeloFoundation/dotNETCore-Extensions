using Pomelo.Data.Excel.Infrastructure;
using System.IO;

namespace Pomelo.Data.Excel
{
    public interface IExcelStreamReader
    {
        ExcelStreamReader Load(Stream stream);

        ExcelStreamReader Load(string path);

        SheetReader LoadSheetReader(string name);

        SheetReader LoadSheetReader(ulong id);
    }
}
