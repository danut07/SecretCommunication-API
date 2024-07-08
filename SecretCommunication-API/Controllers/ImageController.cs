using Microsoft.AspNetCore.Mvc;
using SecretCommunication_API.Models.ImageSteganography.Request;
using SecretCommunication_API.Models.ImageSteganography.Response;
using System.Threading.Tasks.Dataflow;

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
        public async Task<IActionResult> EmbedMessage([FromForm] ImageSteganographyRequest request)
        {
            try
            {
                var resultBytes = await _imageService.EmbedMessageAsync(request.Image, request.Message);
                string extension = Path.GetExtension(request.Image.FileName).ToLower();
                string contentType = extension == ".bmp" ? "image/bmp" : "image/png";
                string fileName = Path.ChangeExtension(request.Image.FileName, extension == ".bmp" ? ".bmp" : ".png");
                return File(resultBytes, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("decode")]
        public async Task<ActionResult<ImageSteganographyResponse>> DecodeMessage([FromForm] IFormFile image)
        {
            try
            {
                var message = await _imageService.DecodeMessageAsync(image);
                return Ok(message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}