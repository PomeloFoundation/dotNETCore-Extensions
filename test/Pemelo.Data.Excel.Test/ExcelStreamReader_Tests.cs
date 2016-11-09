using Pomelo.Data.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pemelo.Data.Excel.Test
{
    public class ExcelStreamReader_Tests
    {
        ExcelStreamReader _excelStreamReader;
        string _excelPath;

        public ExcelStreamReader_Tests()
        {
            _excelStreamReader = new ExcelStreamReader();
            _excelPath = Path.GetTempFileName();

            var excelStream = new ExcelStream();
            if (File.Exists(_excelPath))
            {
                File.Delete(_excelPath);
            }
            using (var x = excelStream.Create(_excelPath))
            {
                using (var sheet = x.LoadSheet(1))
                {
                    sheet.Add(new Pomelo.Data.Excel.Infrastructure.Row
                    {
                        "Name","Sex","Age"
                    });
                    sheet.Add(new Pomelo.Data.Excel.Infrastructure.Row
                    {
                        "Tor","Male","42"
                    });
                    sheet.SaveChanges();
                }
            }
        }

        [Fact]
        public void Test_ExcelStringReader_Load()
        {
            using (var x = _excelStreamReader.Load(_excelPath))
            {
                using (var sheetReader = x.LoadSheetReader(1))
                {
                    // Reading the data from sheet
                    var a = sheetReader.ReadNextRow();
                    while (a != null)
                    {
                        foreach (var b in a)
                        {
                            Assert.Equal("Create test", b);
                        }
                        a = sheetReader.ReadNextRow();
                    }
                }
            }
        }
    }
}
