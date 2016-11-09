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
                    var row = sheetReader.ReadNextRow();
                    var rowCounter = 0;
                    while (row != null)
                    {
                        switch (rowCounter)
                        {
                            case 0:
                                Assert.Equal("Name", row[0]);
                                Assert.Equal("Sex", row[1]);
                                Assert.Equal("Age", row[2]);
                                break;
                            case 1:
                                Assert.Equal("Tor", row[0]);
                                Assert.Equal("Male", row[1]);
                                Assert.Equal("42", row[2]);
                                break;
                        }
                        row = sheetReader.ReadNextRow();
                        rowCounter++;
                    }
                    Assert.Equal(2, rowCounter);
                }
            }
        }

        [Fact]
        public void Test_ExcelStringReader_LoadHdr()
        {
            using (var x = _excelStreamReader.Load(_excelPath))
            {
                using (var sheetReader = x.LoadSheetReaderHDR(1))
                {
                    // Reading the data from sheet
                    var row = sheetReader.ReadNextRow();
                    var rowCounter = 0;
                    while (row != null)
                    {
                        switch (rowCounter)
                        {
                            case 0:
                                Assert.Equal("Tor", row["Name"]);
                                Assert.Equal("Male", row["Sex"]);
                                Assert.Equal("42", row["Age"]);
                                break;
                        }
                        row = sheetReader.ReadNextRow();
                        rowCounter++;
                    }
                    Assert.Equal(1, rowCounter);
                }
            }
        }
    }
}
