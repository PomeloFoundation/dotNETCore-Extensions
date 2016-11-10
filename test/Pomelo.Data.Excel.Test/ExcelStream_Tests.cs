using Pomelo.Data.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pomelo.Data.Excel.Test
{
    public class ExcelStream_Tests
    {
        ExcelStream _excelStream;
        string _excelPath;

        public ExcelStream_Tests()
        {
            _excelStream = new ExcelStream();
            //_excelPath = @"c:\excel\test.xlsx";
            _excelPath = Path.GetTempFileName();
            if (File.Exists(_excelPath))
            {
                File.Delete(_excelPath);
            }
        }

        [Fact(DisplayName = "测试创建Excel文件")]
        public void Test_ExcelStream_Create()
        {
            using (var x = _excelStream.Create(_excelPath))
            {
                using (var sheet = x.LoadSheet(1))
                {
                    sheet.Add(new Pomelo.Data.Excel.Infrastructure.Row
                    {
                        "Create test"
                    });
                    sheet.SaveChanges();
                }
            }
            Assert.True(File.Exists(_excelPath));
        }

        [Fact(DisplayName = "测试通过path加载Excel文件")]
        public void Test_ExcelStream_Load()
        {
            Test_ExcelStream_Create();
            using (var x = _excelStream.Load(_excelPath))
            {
                using (var sheet = x.LoadSheet(1))
                {
                    // Reading the data from sheet
                    foreach (var a in sheet)
                    {
                        foreach (var b in a)
                        {
                            Assert.Equal("Create test", b);
                        }
                    }
                }
            }
        }

        [Fact(DisplayName = "测试修改Excel中的内容")]
        public void Test_ExcelStream_Edit()
        {
            Test_ExcelStream_Create();

            using (var x = _excelStream.Load(_excelPath))
            {
                using (var sheet = x.LoadSheet(1))
                {
                    //Adding a row
                    sheet.Add(new Pomelo.Data.Excel.Infrastructure.Row
                    {
                        null,
                        null,
                        "Hello world!"
                    });
                    //TODO,找到对应行，然后进行删除，再添加

                    sheet.SaveChanges(); // Save changes
                }
            }
        }

        [Fact(DisplayName = "测试通过Stream加载Excel文件")]
        public void Test_ExcelStream_Load_Stream()
        {
            Test_ExcelStream_Create();
            using (var fileStream = new FileStream(_excelPath, FileMode.Open))
            {
                using (var x = _excelStream.Load(fileStream))
                {
                    using (var sheet = x.LoadSheet(1))
                    {
                        // Reading the data from sheet
                        foreach (var row in sheet)
                        {
                            foreach (var value in row)
                            {
                                Assert.Equal(value, "Create test");
                            }
                        }
                    }
                }
            }
        }

        [Fact(DisplayName = "测试创建Excel的Sheet")]
        public void Test_ExcelStream_Create_Sheet()
        {
            Test_ExcelStream_Create();

            using (var x = _excelStream.Load(_excelPath))
            {
                using (var sheet = x.CreateSheet("数据库"))
                {
                    sheet.Add(new Pomelo.Data.Excel.Infrastructure.Row
                    {
                        "Code Comb"
                    });
                    sheet.SaveChanges();
                }
            }
        }

        [Fact(DisplayName = "测试删除Excel中的Sheet")]
        public void Test_ExcelStream_Delete_Sheet()
        {
            Test_ExcelStream_Create_Sheet();

            using (var x = _excelStream.Load(_excelPath))
            {
                x.RemoveSheet("数据库");
            }
        }

        [Fact(DisplayName = "测试第一行作为列索引")]
        public void Test_ExcelStream_LoadSheetHDR()
        {
            using (var x = _excelStream.Create(_excelPath))
            {
                using (var sheet = x.LoadSheet(1))
                {
                    sheet.Add(new Pomelo.Data.Excel.Infrastructure.Row
                    {
                        "Name","Sex","Age"
                    });
                    sheet.Add(new Pomelo.Data.Excel.Infrastructure.Row
                    {
                        "李明","男","12"
                    });
                    sheet.SaveChanges();
                }
            }

            using (var x = _excelStream.Load(_excelPath))
            {
                using (var sheet = x.LoadSheetHDR(1))
                {
                    foreach (var row in sheet)
                    {
                        Assert.Equal(row["Name"], "李明");
                        Assert.Equal(row["Sex"], "男");
                        Assert.Equal(row["Age"], "12");
                    }
                }
            }
        }
    }
}
