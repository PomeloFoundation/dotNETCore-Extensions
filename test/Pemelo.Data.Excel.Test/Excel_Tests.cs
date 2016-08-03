using Pomelo.Data.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pemelo.Data.Excel.Test
{
    public class Excel_Tests
    {
        ExcelStream _ExcelStream;
        string _ExcelPath;

        public Excel_Tests()
        {
            _ExcelStream = new ExcelStream();
            _ExcelPath = @"c:\excel\test.xlsx";

        }

        [Fact(DisplayName = "测试创建Excel文件")]
        public void Test_Creat_Excel()
        {
            if (File.Exists(_ExcelPath))
            {
                File.Delete(_ExcelPath);
            }

            using (var x = _ExcelStream.Create(_ExcelPath))
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
            Assert.True(File.Exists(_ExcelPath));
        }

        [Fact(DisplayName = "测试通过path加载Excel文件")]
        public void Test_Load_Excel()
        {
            Test_Creat_Excel();
            using (var x = _ExcelStream.Load(_ExcelPath))
            {
                using (var sheet = x.LoadSheet(1))
                {
                    // Reading the data from sheet
                    foreach (var a in sheet)
                    {
                        foreach (var b in a)
                        {
                            Assert.Equal(b, "Create test");
                        }
                    }
                }
            }
        }

        [Fact(DisplayName = "测试修改Excel中的内容")]
        public void Test_Edit_Excel()
        {
            Test_Creat_Excel();

            using (var x = _ExcelStream.Load(_ExcelPath))
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
        public void Test_Load_Excel_Stream()
        {
            Test_Creat_Excel();
            using (var fileStream = new FileStream(_ExcelPath, FileMode.Open))
            {
                using (var x = _ExcelStream.Load(fileStream))
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
        public void Test_Create_Sheet()
        {
            Test_Creat_Excel();

            using (var x = _ExcelStream.Load(_ExcelPath))
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
        public void Test_Delete_Sheet()
        {
            Test_Create_Sheet();

            using (var x = _ExcelStream.Load(_ExcelPath))
            {
                x.RemoveSheet("数据库");
            }
        }

        [Fact(DisplayName = "测试第一行作为列索引")]
        public void Test_LoadSheetHDR()
        {
            File.Delete(_ExcelPath);
            using (var x = _ExcelStream.Create(_ExcelPath))
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

            using (var x = _ExcelStream.Load(_ExcelPath))
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
