using Microsoft.AspNetCore.Mvc;

namespace SecretCommunication_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController()
        {
            _imageService = new ImageService();
        }

        [HttpPost("embed")]
        public async Task<IActionResult> EmbedMessage([FromForm] IFormFile image, [FromForm] string message)
        {
            try
            {
                var resultBytes = await _imageService.EmbedMessageAsync(image, message);
                string extension = Path.GetExtension(image.FileName).ToLower();
                string contentType = extension == ".bmp" ? "image/bmp" : "image/png";
                string fileName = Path.ChangeExtension(image.FileName, extension == ".bmp" ? ".bmp" : ".png");
                return File(resultBytes, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("decode")]
        public async Task<IActionResult> DecodeMessage([FromForm] IFormFile image)
        {
            try
            {
                string message = await _imageService.DecodeMessageAsync(image);
                return Ok(message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}