using GerberBackend.Core.DbContext;
using GerberBackend.Core.Dtos.Gerber;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GerberBackend.Core.Entities.Gerber;
using System.Security.Claims;
using GerberBackend.Config;
using GerberBackend.Core.Entities.Gerber.Elements;

namespace GerberBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GerberController : ControllerBase
    {
        private ApplicationContext _context;
        private IHttpContextAccessor _httpContextAccessor;

        public GerberController(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("post-gerber")]
        [Authorize]
        public async Task<IActionResult> AddGerberAsync([FromForm] GerberAddDto dto)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(CustomClaimTypes.Id);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (dto.file == null)
            {
                return BadRequest("File is missing");
            }

            Quantity quantity = new(dto.quantity);
            Count count = new(dto.count);
            Size size = new(dto.sizeX, dto.sizeY);
            BoardWindow window = new(dto.board_window);
            Lamellas lamellas = new Lamellas(dto.lamellas);
            Connectors connectors = new Connectors(dto.quantity);
            Random rand = new Random();
            var price = rand.Next(1000, 5000);
            var order = $"O-{rand.Next(1, 9)}{rand.Next(1, 9)}-{rand.Next(1000000, 9999999)}-P";

            using (var memoryStream = new MemoryStream())
            {
                await dto.file.CopyToAsync(memoryStream);
                var binaryData = new GerberFileBinary
                {
                    Data = memoryStream.ToArray(),
                    Description = dto.file.FileName,
                };
                _context.Gerbers.Add(new OrderGerber
                {
                    BaseMaterialId = dto.materialId + 1,
                    BoardThicknessId = dto.board_thickness + 1,
                    ContourMachiningId = dto.counterId + 1,
                    DataNumberingId = dto.markingDataId + 1,
                    DrillFileId = dto.drillingId + 1,
                    LayerId = dto.layerId + 1,
                    MarkingColorId = dto.markingColorId + 1,
                    MaskColorId = dto.maskColorId + 1,
                    UserId = userId,
                    Count = count,
                    Quantity = quantity,
                    Size = size,
                    BoardWindow = window,
                    AngleChamferId = dto.finishedCorner + 1,
                    EdgeConnectorId = dto.finishedConnectorId + 1,
                    FoilThicknessId = dto.foil_thickness + 1,
                    MarkingSideId = dto.markingSideId + 1,
                    MainSitesId = dto.finishedSitesId + 1,
                    MaskTypeId = dto.maskTypeId + 1,
                    MinimalConductorId = dto.conductorId + 1,
                    MaskSideId = dto.maskSideId + 1,
                    GerberFileBinary = binaryData,
                    ViasId = dto.mask_hole + 1,
                    Lamellas = lamellas,
                    ConnectorsCount = connectors,
                    Status = new Status("Ожидает подтверждения"),
                    Price = new Price(price),
                    Order = new Order(order),
                    BuildTime = new BuildTime(DateTime.SpecifyKind(new DateTime(2024, 7, 20), DateTimeKind.Utc))
                });
                await _context.SaveChangesAsync();
                return Ok();

            }
        }

        [HttpGet("get-full-gerber/{id}")]
        [Authorize]
        public async Task<ActionResult<List<GerberServiceResponseFullListDto>>> GetGerberFullAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(CustomClaimTypes.Id);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var gerberOrders = await _context.Gerbers
            .Where(g => g.UserId == userId && g.Id.ToString().Equals(id))
            .Select(g => new GerberServiceResponseFullListDto
            {
                Id = g.Id.ToString(),
                AngleChamfirdValue = g.AngleChamfer.Value,
                BaseValue = g.BaseMaterial.Value,
                BoardValue = g.BoardThickness.Value,
                BoardWindow = g.BoardWindow.Value.ToString(),
                Connectors = g.ConnectorsCount.Value.ToString(),
                Count = g.Count.Value.ToString(),
                CountourValue = g.ContourMachining.Value.ToString(),
                DataValue = g.DataNumbering.Value,
                DrillValue = g.DrillFile.Value,
                EdgeConductorValue = g.EdgeConnectors.Value,
                FileId = g.GerberFileId,
                FoilValue = g.FoilThickness.Value,
                Lamelass = g.Lamellas.Value.ToString(),
                LayerValue = g.Layer.Value,
                MainSitesValue = g.MainSites.Value,
                MarkingSideValue = g.MarkingSide.Value,
                MarkingValue = g.MarkingColor.Value,
                MaskSideValue = g.MaskSide.Value,
                MaskTypeValue = g.MaskType.Value,
                MaskValue = g.MaskColor.Value,
                MinimalConductorValue = g.MinimalConductor.Value,
                Quantity = g.Quantity.Value.ToString(),
                Size_X = g.Size.X.ToString(),
                Size_Y = g.Size.Y.ToString(),
                ViasValue = g.Vias.Value.ToString(),
                BuildTime = g.BuildTime.Value,
                Order = g.Order.Value,
                Price = g.Price.Value,
                Status = g.Status.Value
            }).AsNoTracking().ToListAsync();

            return Ok(gerberOrders);

        }


        [HttpGet("get-gerber")]
        [Authorize]
        public async Task<ActionResult<List<GerberServiceResponseCompressedListDto>>> GetGerberAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(CustomClaimTypes.Id);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var gerberOrders = await _context.Gerbers
                .Where(g => g.UserId == userId)
                .Select(g => new GerberServiceResponseCompressedListDto
                {
                    Id = g.Id,
                    BuildTime = g.BuildTime.Value,
                    Order = g.Order.Value,
                    Price = g.Price.Value,
                    Status = g.Status.Value
                }).ToListAsync();

            return Ok(gerberOrders);
        }

        [HttpGet("get-gerber-file/{id}")]
        public async Task<IActionResult> GetBinaryData(int id)
        {
            var binaryData = await _context.GerberFileBinaries.FindAsync(id);
            if (binaryData == null)
            {
                return NotFound();
            }

            return File(binaryData.Data, "application/octet-stream", binaryData.Description);
        }
    }
}
