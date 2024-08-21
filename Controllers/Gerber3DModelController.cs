using GerberBackend.Config;
using GerberBackend.Contracts;
using GerberBackend.Core.Contracts;
using GerberBackend.Utils;
using GerberGenerate.Contracts;
using GerberGenerate.Utils;
using Microsoft.AspNetCore.Mvc;
using ILogger = GerberBackend.Core.Contracts.ILogger;

namespace GerberBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Gerber3DModelController : ControllerBase
    {
        private readonly IPullData _data;
        private readonly IGzipData _gzip;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionStore _sessionStore;
        private readonly ILogger _logger;

        private readonly SessionToken _token = new();
        public Gerber3DModelController(IPullData data, IGzipData gzip, IHttpContextAccessor httpContextAccessor, ISessionStore store, ILogger loger)
        {
            _data = data;
            _gzip = gzip;
            _httpContextAccessor = httpContextAccessor;
            _sessionStore = store;
            _logger = loger;
        }

        [HttpPost("get-svg")]
        public async Task<IActionResult> GenerateSvg(IFormFile file)
        {

            GerberStats stats = new GerberStats(file, _data);

            if (file == null)
                return BadRequest();

            var fileName = file.FileName;

            var fileExtension = Path.GetExtension(fileName);

            if (fileExtension != ".zip")
                return BadRequest();

            var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            var httpContext = _httpContextAccessor.HttpContext;

            var user = httpContext.User;

            string sessionId = _token.GetSessionIdFromToken(authHeader);


            (string frontSvg, string backSvg) obj;

            (byte[] frontSvgGzip, byte[] backSvgGzip) svgGzip;

            stats.InitializationFiles();

            stats.GenerateResultFile();

            //obj = PInvokeWrapper.CallProcessPCBFiles(files); 

            //svgGzip.frontSvgGzip = _gzip.CompressString(obj.frontSvg);

            //svgGzip.backSvgGzip = _gzip.CompressString(obj.backSvg);

            GC.Collect();



            //if (sessionId != null) {
            //    var files1 = new Files();
            //    files1.files = svgGzip;
            //    _sessionStore.AddFiles(Guid.Parse(sessionId), files1);
            //    await _logger.LogUserActionAsync(user.FindFirst(CustomClaimTypes.Id)?.Value, "Сгенерировал данные");
            //} else
            //{
            //   await _logger.LogUserActionAsync(null, "Сгенерировал данные");
            //}

            return Ok();
        }

        [HttpPost("get-model")]
        public IActionResult GenerateObj(IFormFile file)
        {
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            string sessionId = _token.GetSessionIdFromToken(authHeader);


            (string mtl, string objModel) obj;

            (byte[] mtlGzip, byte[] objGzip) objGzip;

            var files = _data.getZipFiles(file);

            //obj = PInvokeWrapper.CallGenerateMTLAndOBJFiles(files);

            //objGzip.mtlGzip = _gzip.CompressString(obj.mtl);

            //objGzip.objGzip = _gzip.CompressString(obj.objModel);

            GC.Collect();

            return Ok();
        }
    }
}
