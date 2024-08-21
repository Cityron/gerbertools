using GerberBackend.Contracts;
using System.IO.Compression;
using System.Text;

namespace GerberBackend.Utils;

public class GZipData : IGzipData
{
    public byte[] CompressString(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return outputStream.ToArray();
        }
    }
}
