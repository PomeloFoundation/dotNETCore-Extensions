using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using Pomelo.Data.Excel;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class SheetReader : IDisposable
    {
        private readonly Stream _sheetStream;
        private readonly XmlReader _xmlReader;
        private readonly SharedStrings _sharedStrings;
        private readonly bool _hdr;

        public Header Header { get; private set; }

        public SheetReader(Stream sheetStream, SharedStrings sharedStrings, bool hdr)
        {
            _sheetStream = sheetStream;
            _sharedStrings = sharedStrings;
            _hdr = hdr;

            _xmlReader = XmlReader.Create(sheetStream);

            if (!_xmlReader.ReadToFollowing("sheetData") || !_xmlReader.ReadToFollowing("row"))
            {
                throw new NotSupportedException("Not expected file-format");    
            }

            if (hdr) Header = ReadNextRow();
            if (Header == null) Header = new Header();
        }

        public Row ReadNextRow()
        {
            if (_xmlReader.EOF) return null;

            var row = new Row(Header);

            var moreColumns = true;
            var elements = new [] { "c"}; 
            while (!_xmlReader.EOF && moreColumns)
            {
                string type = null;
                string pos = null;

                while (!_xmlReader.EOF && moreColumns && SkipIrrelevant(elements))
                {
                    switch (_xmlReader.Name)
                    {
                        case "c":
                            pos = _xmlReader.GetAttribute("r");
                            type = _xmlReader.GetAttribute("t");
                            switch (type)
                            {
                                case "inlineStr":
                                    elements = new [] { "c", "is", "t", "row"};
                                    break;
                                default:
                                    elements = new [] { "c", "v", "row" };
                                    break;
                            }
                            if (!_xmlReader.EOF) _xmlReader.Read();
                            break;
                        case "is":
                            if (!_xmlReader.EOF) _xmlReader.Read();
                            break;
                        case "v":
                        case "t":
                            var value = _xmlReader.ReadInnerXml();
                            if (!_xmlReader.EOF) _xmlReader.Read();

                            if (type == "s")
                            {
                                int index;
                                if (int.TryParse(value, out index))
                                {
                                    value = _sharedStrings[index];
                                }
                            }

                            row.Add(value, pos);

                            break;
                        case "row":
                            moreColumns = false;
                            break;
                    }
                }
            }

            return row;
        }

        private bool SkipIrrelevant(string[] elements)
        {
            while (!_xmlReader.EOF)
            {
                foreach (string element in elements)
                {
                    if (_xmlReader.NodeType == XmlNodeType.Element && _xmlReader.Name == element)
                    {
                        return !_xmlReader.EOF;
                    }
                }

                _xmlReader.Read();
            }

            return !_xmlReader.EOF; ;
        }

        public void Dispose()
        {
            _sheetStream?.Dispose();
            _xmlReader?.Dispose();

            GC.Collect();
        }
    }
}
