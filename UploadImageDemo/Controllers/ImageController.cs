using Microsoft.AspNetCore.Mvc;
using UploadImageDemo.Services;

namespace UploadImageDemo.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        #region Property
        private readonly ILogger<ImageController> _logger;
        private readonly IImageService _imageService;
        #endregion

        #region Constructor
        public ImageController(ILogger<ImageController> logger, IImageService imageService)
        {
            this._imageService = imageService;
            this._logger = logger;
        }
        #endregion

        #region Action
        [HttpPost]
        public async Task<IActionResult> UploadImageAsync(IFormFile image)
        {
            if (image is null || image.Length == 0)
                return BadRequest();
            if (image.Length > 5242880) // More than 5 MB
                return BadRequest();

            var filePath = Path.GetTempFileName();

            var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            stream.Position = 0;

            var result = await _imageService.UploadImageAsync(stream);

            stream.Dispose();

            // Clean temp-file
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return result.isSuccess ? Ok(result.uri) : BadRequest();
        }
        #endregion
    }
}
