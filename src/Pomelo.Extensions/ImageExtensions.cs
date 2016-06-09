using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public class Base64StringImage
    {
        public Base64StringImage() { }

        public Base64StringImage(byte[] Image, string contentType)
        {
            ContentString = Convert.ToBase64String(Image);
            ContentType = contentType;
        }

        public Base64StringImage(string ImageString)
        {
            ContentString = ImageString.Split(',')[1].Trim();
            ContentType = ImageString.Split(':')[1].Split(';')[0];
        }

        public string ContentType { get; set; }

        public string ContentString { get; set; }

        public byte[] AllBytes
        {
            get { return Convert.FromBase64String(ContentString); }
        }

        public string ImageString
        {
            get { return "data:" + ContentType + "; base64, " + ContentString; }
        }
    }

}
