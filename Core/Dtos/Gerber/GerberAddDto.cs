using Microsoft.Extensions.FileProviders;

namespace GerberBackend.Core.Dtos.Gerber;

public record GerberAddDto(int materialId, int layerId, int sizeX, int sizeY,
    int count,
    int board_thickness, int foil_thickness, int board_window, int lamellas,
    int quantity, int maskTypeId, int maskSideId, int maskColorId, int markingSideId, int markingColorId,
    int markingDataId, int finishedSitesId, int finishedConnectorId, int finishedCorner, int conductorId, int counterId, int drillingId, int mask_hole, IFormFile file);
