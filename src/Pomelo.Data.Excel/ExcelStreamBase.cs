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
    public class ExcelStreamBase<T> where T : ExcelStreamBase<T>, IDisposable
    {
        protected Stream FileStream { get; set; }

        protected SharedStrings SharedStrings { get; set; }

        public ZipArchive ZipArchive { get; set; }

        public List<WorkBook> WorkBook { get; set; } = new List<WorkBook>();

        public ExcelStreamBase()
        {

        }

        public T Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            FileStream = null;
            ZipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
            ReadFromZip();

            return (T)this;
        }

        /// <summary>
        /// 加载Excel
        /// </summary>
        /// <param name="path">Excel文件路径</param>
        public T Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{nameof(path)}is not found");
            }
            FileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            ZipArchive = new ZipArchive(FileStream, ZipArchiveMode.Read | ZipArchiveMode.Update);
            ReadFromZip();

            return (T)this;
        }

        public void Dispose()
        {
            ZipArchive?.Dispose();
            FileStream?.Dispose();
            SharedStrings?.Dispose();
        }

        private void ReadFromZip()
        {
            var e = ZipArchive.GetEntry("xl/_rels/workbook.xml.rels");
            using (var streamRels = e.Open())
            {
                var sr = new StreamReader(streamRels);
                var result = sr.ReadToEnd();
                var xdRels = new XmlDocument();
                xdRels.LoadXml(result);

                e = ZipArchive.GetEntry("xl/workbook.xml");
                using (var stream = e.Open())
                {
                    sr = new StreamReader(stream);
                    result = sr.ReadToEnd();
                    var xd = new XmlDocument();
                    xd.LoadXml(result);
                    var tmp = xd.GetElementsByTagName("sheet");
                    foreach (XmlNode x in tmp)
                    {
                        var name = x.Attributes["name"].Value;
                        var sheetId = x.Attributes["sheetId"].Value;
                        var rId = x.Attributes["r:id"].Value;

                        var relationship =
                            xdRels
                            .GetElementsByTagName("Relationship")
                            .Cast<XmlNode>()
                            .Single(x2 => x2.Attributes["Id"].Value == rId);

                        var target = relationship.Attributes["Target"].Value;

                        WorkBook.Add(new WorkBook
                        {
                            Name = name,
                            Id = Convert.ToUInt64(sheetId),
                            Target = target
                        });
                    }
                }
            }
        }

        protected SharedStrings CachedSharedStrings
        {
            get
            {
                if (SharedStrings == null)
                {
                    var e = ZipArchive.GetEntry("xl/sharedStrings.xml");
                    // 如果sharedStrings.xml不存在，则创建
                    if (e == null 
                        && ((ZipArchive.Mode & ZipArchiveMode.Update) == ZipArchiveMode.Update) 
                        && GetType() != typeof(ExcelStreamReader))
                    {
                        // 创建sharedStrings.xml
                        e = ZipArchive.CreateEntry("xl/sharedStrings.xml", CompressionLevel.Optimal);
                        using (var stream = e.Open())
                        using (var sw = new StreamWriter(stream))
                        {
                            sw.Write(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><sst xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" count=""0""></sst>");
                        }

                        // 同时需要向[Content_Types].xml添加sharedStrings.xml的信息
                        var e2 = ZipArchive.GetEntry("[Content_Types].xml");
                        using (var stream = e2.Open())
                        using (var sr = new StreamReader(stream))
                        {
                            var result = sr.ReadToEnd();
                            var xd = new XmlDocument();
                            xd.LoadXml(result);
                            var element = xd.CreateElement("Override", xd.DocumentElement.NamespaceURI);
                            element.SetAttribute("PartName", "/xl/sharedStrings.xml");
                            element.SetAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml");
                            xd.GetElementsByTagName("Types")
                                .Cast<XmlNode>()
                                .First()
                                .AppendChild(element);
                            stream.Position = 0;
                            stream.SetLength(0);
                            xd.Save(stream);
                        }

                        // 还需要向xl rels添加
                        var e3 = ZipArchive.GetEntry("xl/_rels/workbook.xml.rels");
                        using (var stream = e3.Open())
                        using (var sr = new StreamReader(stream))
                        {
                            var result = sr.ReadToEnd();
                            var xd = new XmlDocument();
                            xd.LoadXml(result);
                            var tmp = xd.GetElementsByTagName("Relationships")
                                .Cast<XmlNode>()
                                .First();
                            var element = xd.CreateElement("Relationship", xd.DocumentElement.NamespaceURI);
                            element.SetAttribute("Target", "sharedStrings.xml");
                            element.SetAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings");
                            element.SetAttribute("Id", $"rId{tmp.ChildNodes.Count + 1}");
                            tmp.AppendChild(element);
                            stream.Position = 0;
                            stream.SetLength(0);
                            xd.Save(stream);
                        }
                    }
                   
                    if (e != null)
                    {
                        using (var stream = e.Open())
                        {
                            var sr = new StreamReader(stream);
                            var result = sr.ReadToEnd();
                            SharedStrings = new SharedStrings(result);
                        }
                    }
                }
                return SharedStrings;
            }
        }
    }
}
