using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using CodeComb.Data.Excel;

namespace CodeComb.Data.Excel.Infrastructure
{
    public class Sheet : List<Row>, IDisposable
    {
        public Sheet(ulong Id, ExcelStream Excel, SharedStrings StringDictionary)
        {
            this.Id = Id;
            this.StringDictionary = StringDictionary;
            this.excel = Excel;
        }

        private ExcelStream excel;

        private ulong Id;

        protected SharedStrings StringDictionary { get; set; }

        public void Dispose()
        {
            this.Clear();
            GC.Collect();
        }

        public void SaveChanges()
        {
            var tmp = this.Cast<List<string>>()
                .ToList();
            if (this is SheetHDR)
                tmp.Insert(0, (this as SheetHDR).Header.ToList());
            var row = 1ul;
            ColNumber col;
            // 获取sheetX.xml
            var sheetX = excel.ZipArchive.GetEntry($"xl/worksheets/sheet{Id}.xml");
            using (var stream = sheetX.Open())
            using (var sr = new StreamReader(stream))
            {
                // 获取sheetData节点
                var xd = new XmlDocument();
                xd.LoadXml(sr.ReadToEnd());
                var sheetData = xd.GetElementsByTagName("sheetData")
                    .Cast<XmlNode>()
                    .First();
                // 删除全部元素
                sheetData.RemoveAll();
                foreach (var x in this)
                {
                    col = new ColNumber("A");

                    // 添加row节点
                    var element = xd.CreateElement("row", xd.DocumentElement.NamespaceURI);
                    element.SetAttribute("r", row.ToString());
                    element.SetAttribute("spans", "1:1");
                    foreach (var y in x)
                    {
                        var innerText = "";
                        var flag = false;
                        try
                        {
                            if (y.Length > 11 && y[0] != '-' || y.Length > 12 && y[0] == '-')
                                throw new Exception();
                            // 如果是数值类型，则直接写入xml
                            if (x.Contains("."))
                            {
                                // 如果是小数尝试转换为double
                                innerText = Convert.ToDouble(y).ToString();
                            }
                            else
                            {
                                // 如果是整数尝试转换为long
                                innerText = Convert.ToInt64(y).ToString(); 
                            }
                        }
                        catch
                        {
                            // 否则需要将字符串添加到sharedStrings.xml中，并生成索引
                            if (!StringDictionary.Exist(y))
                                innerText = StringDictionary._Add(y).ToString();
                            else
                                innerText = StringDictionary._IndexOf(y).ToString();
                            flag = true;
                        }
                        var element2 = xd.CreateElement("c", xd.DocumentElement.NamespaceURI);
                        element2.SetAttribute("r", col + row.ToString());
                        if (flag)
                            element2.SetAttribute("t", "s");
                        var element3 = xd.CreateElement("v", xd.DocumentElement.NamespaceURI);
                        element3.InnerText = innerText;
                        element2.AppendChild(element3);
                        element.AppendChild(element2);
                        col++;
                    }
                    sheetData.AppendChild(element);
                    row++;
                }
                // 保存sheetX.xml
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }
            // 回收垃圾
            GC.Collect();

            // 保存sharedStrings.xml
            var sharedStrings = excel.ZipArchive.GetEntry($"xl/sharedStrings.xml");
            using (var stream = sharedStrings.Open())
            using (var sw = new StreamWriter(stream))
            {
                var xmlString = new StringBuilder($@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<sst count=""{StringDictionary.LongCount()}"" xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"">");
                var inner = new StringBuilder();
                foreach (var x in StringDictionary)
                    xmlString.Append($"    <si><t>{x}</t></si>\r\n");
                xmlString.Append("</sst>");
                sw.Write(xmlString);
            }
            // 回收垃圾
            GC.Collect();
        }
    }
}
