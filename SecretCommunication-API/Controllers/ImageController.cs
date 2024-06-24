using Microsoft.AspNetCore.Mvc;
using SecretCommunication.BusinessLayer.Interface;
using SecretCommunication_API.Models.ImageSteganography;
using SecretCommunication_API.Utils;

namespace SecretCommunication_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : BaseApiController
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("embed")]
        public async Task<IActionResult> EmbedMessage([FromForm] IFormFile image, [FromForm] string message)
        {
            try
            {
                var resultBytes = await _imageService.EmbedMessageAsync(image, message);
                return File(resultBytes, "image/png", "embedded_image.png");
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