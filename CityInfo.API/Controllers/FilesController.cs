using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers {
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FilesController : ControllerBase {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(
            FileExtensionContentTypeProvider fileExtensionContentTypeProvider) {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
                ?? throw new System.ArgumentNullException(
                    nameof(fileExtensionContentTypeProvider));
        }

        [HttpGet("{fileId}")]
        public IActionResult GetFile(int fileId) {
            // look up the actual file, depending on the fileId...
            // demo code
            var pathToFile = "theory-lectures.pdf";

            // check whether the file exists
            if (!System.IO.File.Exists(pathToFile)) {
                return NotFound();
            }

            if (!_fileExtensionContentTypeProvider.TryGetContentType(
               pathToFile, out var contentType)) {
                contentType = "application/octet-stream"; // if the file type canot be find it will be set as octet-stream
            }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);
            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }
    }
}
