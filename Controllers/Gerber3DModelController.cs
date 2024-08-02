using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerberBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Gerber3DModelController : ControllerBase
    {
        [HttpPost("get-model")]
        [Authorize]
        public async Task<IActionResult> Generate3DModel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var gltfFilePath = Path.Combine(Environment.CurrentDirectory, "output.gltf");
            var binFilePath = Path.Combine(Environment.CurrentDirectory, "output.bin");
            var png1File = Path.Combine(Environment.CurrentDirectory, "png.png");
            var png2File = Path.Combine(Environment.CurrentDirectory, "png.png");

            if (!System.IO.File.Exists(gltfFilePath) || !System.IO.File.Exists(binFilePath))
            {
                return NotFound("File not found.");
            }

            var gltfBase64 = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(gltfFilePath));
            var binBase64 = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(binFilePath));
            var png1Base64 = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(png1File));
            var png2Base64 = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(png2File));
            var classt = 11;
            var SizeX = 99;
            var SizeY = 99;

            var result = new
            {
                gltfFile = gltfBase64,
                binFile = binBase64,
                png1File = png1Base64,
                png2File = png2Base64,
                classt = classt,
                SizeX = SizeX,
                SizeY = SizeY
            };

            return Ok(result);
        }
    }
}
