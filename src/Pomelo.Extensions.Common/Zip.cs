using System.IO;
using System.IO.Compression;

namespace Pomelo.Package
{
    public static class Zip
    {
        public static void Compress(string path, string dest, CompressionLevel level = CompressionLevel.Optimal)
        {
            using (var stream = new FileStream(dest, FileMode.OpenOrCreate))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach(var x in files)
                {
                    var entry = archive.CreateEntry(x.Replace(path, "").Trim('/').Trim('\\'), level);
                    using (StreamWriter writer = new StreamWriter(entry.Open()))
                    {
                        var file = new FileStream(x, FileMode.Open);
                        file.CopyTo(writer.BaseStream);
                    }
                }
            }
        }
    }
}
