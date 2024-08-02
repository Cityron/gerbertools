namespace GerberBackend.Core.Dtos.Gerber;

public class GerberServiceResponseFullListDto
{
    public string Id { get; init; }
    public string Size_X { get; init; }
    public string Size_Y { get; init; }
    public string Count { get; init; }
    public string Quantity { get; init; }
    public string Order { get; init; }
    public DateTime BuildTime { get; init; }
    public int Price { get; init; }
    public string Status { get; init; }
    public string BoardWindow {  get; init; }
    public string Lamelass { get; init; }
    public string Connectors { get; init; }
    public string MaskValue { get; init; }
    public string MarkingValue { get; init; }
    public string DataValue { get; init; }
    public string CountourValue { get; init; }
    public string BaseValue { get; init; }
    public string LayerValue { get; init; }
    public string BoardValue { get; init; }
    public string DrillValue { get; init; }
    public string MarkingSideValue { get; init; }
    public string MaskTypeValue { get; init; }
    public string MaskSideValue { get; init; }
    public string ViasValue { get; init; }
    public string MainSitesValue { get; init; }
    public string EdgeConductorValue { get; init; }
    public string MinimalConductorValue { get; init; }
    public string AngleChamfirdValue { get; init; }
    public string FoilValue { get; init; }
    public int FileId { get; init; }
}
