using GerberBackend.Core.Entities.Auth;
using GerberBackend.Core.Entities.Gerber.Elements;
using System.ComponentModel.DataAnnotations;

namespace GerberBackend.Core.Entities.Gerber;

public class OrderGerber
{
    [Key]
    public int Id { get; init; }

    public Size Size { get; init; }

    public Quantity Quantity { get; init; }

    public Count Count { get; init; }

    public BuildTime BuildTime { get; init; }

    public Status Status { get; init; }

    public Order Order { get; init; }

    public Price Price { get; init; }

    public BoardWindow BoardWindow { get; init; }

    public Lamellas Lamellas { get; init; }

    public Connectors ConnectorsCount { get; init; }

    public int MaskColorId { get; init; }

    public int MarkingColorId { get; init; }

    public int DataNumberingId { get; init; }

    public int ContourMachiningId { get; init; }

    public int BaseMaterialId { get; init; }

    public int LayerId { get; init; }

    public int BoardThicknessId { get; init; }

    public int DrillFileId { get; init; }

    public int GerberFileId { get; init; }

    public int MarkingSideId { get; init; }

    public int MaskTypeId { get; init; }

    public int MaskSideId { get; init; }

    public int ViasId { get; init; }

    public int MainSitesId { get; init; }

    public int EdgeConnectorId { get; init; }

    public int MinimalConductorId {  get; init; }

    public int AngleChamferId { get; init; }

    public int FoilThicknessId { get; init; }

    public string UserId { get; init; }




    public ApplicationUser User { get; init; }

    public MaskColor MaskColor { get; init; }

    public MarkingColor MarkingColor { get; init; }

    public DataNumbering DataNumbering { get; init; }

    public ContourMachining ContourMachining { get; init; }

    public BaseMaterial BaseMaterial { get; init; }

    public Layer Layer { get; init; }

    public BoardThickness BoardThickness { get; init; }

    public FoilThickness FoilThickness { get; init; }

    public DrillFile DrillFile { get; init; }

    public GerberFileBinary GerberFileBinary { get; init; }

    public MaskType MaskType { get; init; }

    public MaskSide MaskSide { get; init; }

    public MarkingSide MarkingSide { get; init; }

    public MinimalConductor MinimalConductor { get; init; }

    public EdgeConnectors EdgeConnectors { get; init; }

    public Vias Vias { get; init; }

    public MainSites MainSites { get; init; }

    public AngleChamfer AngleChamfer { get; init; }

}
