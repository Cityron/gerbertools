namespace GerberBackend.Core.Dtos.Gerber;

public class GerberServiceResponseCompressedListDto
{
    public int Id { get; init; }

    public string Order { get; init; }

    public DateTime BuildTime { get; init; }

    public int Price { get; init; }

    public string Status { get; init; }
}
