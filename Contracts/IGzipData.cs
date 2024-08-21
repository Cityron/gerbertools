namespace GerberBackend.Contracts;

public interface IGzipData
{
    public byte[] CompressString(string text);
}
