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
                string fileName = Path.ChangeExtension(image.FileName, ".png");
                return File(resultBytes, "image/png", fileName);
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