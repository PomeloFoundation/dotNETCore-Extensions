using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class SheetWithoutHDR : Sheet
    {
        public SheetWithoutHDR(ulong id, string xmlSource, ExcelStream excel, SharedStrings stringDictionary)
            : base(id, excel, stringDictionary)
        {
            var xd = new XmlDocument();
            xd.LoadXml(xmlSource);
            var rows = xd.GetElementsByTagName("row");
            // 遍历row标签
            foreach (XmlNode x in rows)
            {
                var cols = x.ChildNodes;
                var objs = new Row();
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

                    objs.Add(value, y.Attributes["r"].Value);
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