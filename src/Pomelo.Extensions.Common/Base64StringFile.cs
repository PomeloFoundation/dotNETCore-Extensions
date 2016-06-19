namespace System
{
    public class Base64StringFile
    {
        public Base64StringFile() { }

        public Base64StringFile(byte[] Blob, string contentType)
        {
            ContentString = Convert.ToBase64String(Blob);
            ContentType = contentType;
        }

        public Base64StringFile(string base64String)
        {
            ContentString = base64String.Split(',')[1].Trim();
            ContentType = base64String.Split(':')[1].Split(';')[0];
        }

        public string ContentType { get; set; }

        private string ContentString { get; set; }

        private byte[] allBytes;

        public byte[] AllBytes
        {
            get
            {
                if (allBytes == null)
                {
                    allBytes = Convert.FromBase64String(ContentString);
                }
                return allBytes;
            }
        }

        private string base64String;

        public string Base64String
        {
            get
            {
                if (base64String == null)
                {
                    base64String = "data:" + ContentType + "; base64, " + ContentString;
                }
                return base64String;
            }
        }
    }

}
