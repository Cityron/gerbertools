namespace GerberBackend.Core.Entities.Gerber.Elements;

public class GerberFileBinary
{
    public int Id { get; init; }

    public byte[] Data { get; init; }

    public string Description { get; init; }

    public ICollection<OrderGerber> OrderGerbers { get; init; }
}
