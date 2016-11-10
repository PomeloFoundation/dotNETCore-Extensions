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
    public class ExcelStream : ExcelStreamBase<ExcelStream>, IExcelStream, IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelStream()
        {

        }

        /// <summary>
        /// 创建ExcelStream
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public ExcelStream Create(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            //check the dirctory
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(path, NewExcel.Bytes);
            Load(path);
            return this;
        }

        /// <summary>
        /// 创建Sheet不包含头部列
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        public SheetWithoutHDR CreateSheet(string name)
        {
            var id = createSheet(name);
            return LoadSheet(id);
        }

        /// <summary>
        /// 创建Sheet包含头部列
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        public SheetHDR CreateSheetHDR(string name)
        {
            var id = createSheet(name);
            return LoadSheetHDR(id);
        }

        /// <summary>
        /// 加载Sheet不包含头部
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        public SheetWithoutHDR LoadSheet(string name)
        {
            var id = WorkBook
                .Where(x => x.Name == name)
                .Select(x => x.Id)
                .First();
            return LoadSheet(id);
        }

        /// <summary>
        /// 加载Sheet不包含头部
        /// </summary>
        /// <param name="id">Sheet名称对应的Id</param>
        /// <returns></returns>
        public SheetWithoutHDR LoadSheet(ulong id)
        {
            var worksheet = WorkBook.First(x => x.Id == id);
            var e = ZipArchive.GetEntry($"xl/{worksheet.Target}");
            using (var stream = e.Open())
            {
                var sr = new StreamReader(stream);
                var result = sr.ReadToEnd();
                return new SheetWithoutHDR(worksheet.Id, result, this, CachedSharedStrings);
            }
        }

        /// <summary>
        /// 加载Sheet包含头部
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        public SheetHDR LoadSheetHDR(string name)
        {
            var id = WorkBook
                .Where(x => x.Name == name)
                .Select(x => x.Id)
                .First();
            return LoadSheetHDR(id);
        }

        /// <summary>
        /// 加载Sheet包含头部
        /// </summary>
        /// <param name="Id">Sheet名称对应的Id</param>
        /// <returns></returns>
        public SheetHDR LoadSheetHDR(ulong Id)
        {
            var worksheet = WorkBook.Where(x => x.Id == Id).First();
            var e = ZipArchive.GetEntry($"xl/{worksheet.Target}");
            using (var stream = e.Open())
            {
                var sr = new StreamReader(stream);
                var result = sr.ReadToEnd();
                return new SheetHDR(worksheet.Id, result, this, CachedSharedStrings);
            }
        }

        /// <summary>
        /// 删除Sheet
        /// </summary>
        /// <param name="name">Sheet名称</param>
        public void RemoveSheet(string name)
        {
            var sheetId = WorkBook.Where(x => x.Name == name)
                .Select(x => x.Id)
                .First();
            RemoveSheet(sheetId);
        }

        /// <summary>
        /// 删除Sheet
        /// </summary>
        /// <param name="Id">Sheet名称对应的Id</param>
        public void RemoveSheet(ulong Id)
        {
            var name = WorkBook.Where(x => x.Id == Id).First().Name;
            // 从ExcelStream对象中删除
            WorkBook.Remove(WorkBook.Where(x => x.Id == Id).First());

            // 从workbook.xml中删除
            var e = ZipArchive.GetEntry("xl/workbook.xml");
            using (var stream = e.Open())
            {
                var sr = new StreamReader(stream);
                var result = sr.ReadToEnd();
                var xd = new XmlDocument();
                xd.LoadXml(result);
                var tmp = xd
                    .GetElementsByTagName("sheet")
                    .Cast<XmlNode>()
                    .Single(x => x.Attributes["sheetId"].Value == Id.ToString());
                tmp.ParentNode.RemoveChild(tmp);
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }

            // 删除sheetX.xml
            var e2 = ZipArchive.GetEntry($"xl/worksheets/sheet{Id}.xml");
            e2.Delete();

            // 从[Content_Types].xml中删除
            var e3 = ZipArchive.GetEntry("[Content_Types].xml");
            using (var stream = e3.Open())
            {
                var sr = new StreamReader(stream);
                var result = sr.ReadToEnd();
                var xd = new XmlDocument();
                xd.LoadXml(result);
                var tmp = xd
                    .GetElementsByTagName("Override")
                    .Cast<XmlNode>()
                    .Single(x => x.Attributes["PartName"].Value == $"/xl/worksheets/sheet{Id}.xml");
                tmp.ParentNode.RemoveChild(tmp);
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }

            // 从app.xml中移除
            var e4 = ZipArchive.GetEntry("docProps/app.xml");
            using (var stream = e4.Open())
            using (var sr = new StreamReader(stream))
            {
                var result = sr.ReadToEnd();
                var xd = new XmlDocument();
                xd.LoadXml(result);
                var tmp = xd
                    .GetElementsByTagName("vt:lpstr")
                    .Cast<XmlNode>()
                    .Single(x => x.InnerText == name);
                tmp.ParentNode.Attributes["size"].Value = (Convert.ToInt32(tmp.ParentNode.Attributes["size"].Value) - 1).ToString();
                tmp.ParentNode.RemoveChild(tmp);
                var tmp2 = xd.GetElementsByTagName("vt:i4")
                    .Cast<XmlNode>()
                    .Single();
                tmp2.InnerText = (Convert.ToInt32(tmp2.InnerText) - 1).ToString();
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }

            // 重新整理xl/rels
            var e5 = ZipArchive.GetEntry("xl/_rels/workbook.xml.rels");
            using (var stream = e5.Open())
            using (var sr = new StreamReader(stream))
            {
                var result = sr.ReadToEnd();
                var xd = new XmlDocument();
                xd.LoadXml(result);
                var relationships = xd.GetElementsByTagName("Relationships")
                    .Cast<XmlNode>()
                    .First();
                var sheetX = relationships.ChildNodes
                    .Cast<XmlNode>()
                    .Single(x => x.Attributes["Target"].Value == $"worksheets/sheet{Id}.xml");
                relationships.RemoveChild(sheetX);
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }
        }


        public void Dispose()
        {
            ZipArchive.Dispose();
            FileStream?.Dispose();
        }

        #region 私有方法

        private ulong createSheet(string name)
        {
            var Id = WorkBook.Count > 0 ? WorkBook.Max(x => x.Id) + 1 : 1;
            // 向缓存中添加该Id
            WorkBook.Add(new WorkBook
            {
                Id = Id,
                Name = name,
                //修复创建Sheet的无法找到Root的Bug
                Target = $"worksheets/sheet{Id}.xml"
            });

            // 添加sheetX.xml
            var e = ZipArchive.CreateEntry($"xl/worksheets/sheet{Id}.xml", CompressionLevel.Optimal);
            using (var stream = e.Open())
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:xdr=""http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"" xmlns:x14=""http://schemas.microsoft.com/office/spreadsheetml/2009/9/main"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"">
  <sheetPr />
  <dimension ref=""A1"" />
  <sheetViews>
    <sheetView workbookViewId=""0"">
      <selection activeCell=""A1"" sqref=""A1"" />
    </sheetView>
  </sheetViews>
  <sheetFormatPr defaultColWidth=""9"" defaultRowHeight=""13.5"" />
  <sheetData>
  </sheetData>
  <pageMargins left=""0.75"" right=""0.75"" top=""1"" bottom=""1"" header=""0.511805555555556"" footer=""0.511805555555556"" />
  <headerFooter />
</worksheet>");
            }

            // 向[Content_Types].xml中添加sheetX.xml
            var e2 = ZipArchive.GetEntry("[Content_Types].xml");
            using (var stream = e2.Open())
            using (var sr = new StreamReader(stream))
            {
                var xd = new XmlDocument();
                xd.LoadXml(sr.ReadToEnd());
                var element = xd.CreateElement("Override", xd.DocumentElement.NamespaceURI);
                element.SetAttribute("PartName", $"/xl/worksheets/sheet{Id}.xml");
                element.SetAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
                var tmp = xd.GetElementsByTagName("Types")
                                .Cast<XmlNode>()
                                .First()
                                .AppendChild(element);
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }

            string identifier = "rId";

            // 向xl/rels中添加
            var e5 = ZipArchive.GetEntry("xl/_rels/workbook.xml.rels");
            using (var stream = e5.Open())
            using (var sr = new StreamReader(stream))
            {
                var result = sr.ReadToEnd();
                var xd = new XmlDocument();
                xd.LoadXml(result);
                var relationships = xd.GetElementsByTagName("Relationships")
                    .Cast<XmlNode>()
                    .First();
                identifier += (relationships.ChildNodes.Count + 1).ToString();
                var element = xd.CreateElement("Relationship", xd.DocumentElement.NamespaceURI);
                element.SetAttribute("Target", $"worksheets/sheet{Id}.xml");
                element.SetAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet");
                element.SetAttribute("Id", identifier);
                relationships.AppendChild(element);
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }

            // 向workbook.xml添加sheetX.xml
            var e3 = ZipArchive.GetEntry("xl/workbook.xml");
            using (var stream = e3.Open())
            using (var sr = new StreamReader(stream))
            {
                var xd = new XmlDocument();
                xd.LoadXml(sr.ReadToEnd());
                var tmp = xd.GetElementsByTagName("sheets")
                                .Cast<XmlNode>()
                                .First();
                var element = xd.CreateElement("sheet", xd.DocumentElement.NamespaceURI);
                tmp.AppendChild(element);
                var attr = xd.CreateAttribute("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                attr.Value = identifier;
                element.Attributes.Append(attr);
                element.SetAttribute("sheetId", Id.ToString());
                element.SetAttribute("name", name);
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }

            // 向app.xml中添加sheetX.xml
            var e4 = ZipArchive.GetEntry("docProps/app.xml");
            using (var stream = e4.Open())
            using (var sr = new StreamReader(stream))
            {
                var result = sr.ReadToEnd();
                var xd = new XmlDocument();
                xd.LoadXml(result);
                var element = xd.CreateElement("vt:lpstr", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
                element.InnerText = name;
                var tmp = xd
                    .GetElementsByTagName("vt:vector")
                    .Cast<XmlNode>()
                    .Single(x => x.Attributes["baseType"].Value == "lpstr");
                tmp.AppendChild(element);

                tmp.Attributes["size"].Value = (Convert.ToInt32(tmp.Attributes["size"].Value) + 1).ToString();

                var tmp2 = xd.GetElementsByTagName("vt:i4")
                    .Cast<XmlNode>()
                    .Single();
                tmp2.InnerText = (Convert.ToInt32(tmp2.InnerText) + 1).ToString();
                stream.Position = 0;
                stream.SetLength(0);
                xd.Save(stream);
            }

            return Id;
        }
        #endregion
    }
}
