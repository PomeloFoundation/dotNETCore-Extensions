using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class SheetHDR : Sheet
    {
        public Header Header { get; private set; }

        public SheetHDR(ulong Id, string XmlSource, ExcelStream Excel, SharedStrings stringDictionary)
            :base(Id, Excel, stringDictionary)
        {
            var xd = new XmlDocument();
            xd.LoadXml(XmlSource);
            var rows = xd.GetElementsByTagName("row");
            // 遍历row标签
            var flag = false;
            Header = new Header();
            foreach (XmlNode x in rows)
            {
                var cols = x.ChildNodes;
                var objs = new Row(Header);
                // 遍历c标签
                foreach (XmlNode y in cols)
                {
                    string value = null;
                    // 如果是字符串类型，则需要从字典中查询
                    if (y.Attributes["t"]?.Value == "s")
                    {
                        var index = Convert.ToUInt64(y.FirstChild.InnerText);
                        value = StringDictionary[index];
                    }
                    else if (y.Attributes["t"]?.Value == "inlineStr")
                    {
                        value = y.FirstChild.FirstChild.InnerText;
                    }
                    // 否则其中的v标签值即为单元格内容
                    else
                    {
                        value = y.InnerText;
                    }

                    if (!flag)
                    {
                        Header.Add(value, y.Attributes["r"].Value);
                        continue;
                    }
                    objs.Add(value, y.Attributes["r"].Value);
                }
                if (!flag)
                {
                    while (Header.LastOrDefault() == null)
                        Header.RemoveAt(Header.Count - 1);
                    flag = true;
                    continue;
                }
                // 去掉末尾的null
                while (objs.LastOrDefault() == null)
                    objs.RemoveAt(objs.Count - 1);
                if (objs.Count > 0)
                    this.Add(objs);
            }
            while (this.Count > 0 && this.Last().Count == 0)
                this.RemoveAt(this.Count - 1);
            GC.Collect();
        }
    }
}
