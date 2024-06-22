using Microsoft.AspNetCore.Mvc;
using SecretCommunication.BusinessLayer.Interface;

namespace SecretCommunication_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            var processedImage = await _imageService.ProcessImageAsync(image);
            return File(processedImage, "application/octet-stream");
        }

        [HttpPost("embed")]
        public async Task<IActionResult> EmbedMessage(IFormFile image, string message)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            if (string.IsNullOrEmpty(message))
                return BadRequest("No message provided.");

            var modifiedImage = await _imageService.EmbedMessageAsync(image, message);
            return File(modifiedImage, "application/octet-stream");
        }

        [HttpPost("extract")]
        public async Task<IActionResult> ExtractMessage(IFormFile image, int messageLength)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            if (messageLength <= 0)
                return BadRequest("Invalid message length.");

            var message = await _imageService.ExtractMessageAsync(image, messageLength);
            return Ok(message);
        }
    }
}