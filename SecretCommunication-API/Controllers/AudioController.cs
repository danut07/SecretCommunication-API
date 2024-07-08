using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SecretCommunication_API.BusinessLayer;

namespace SecretCommunication_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        private readonly AudioService _audioService;

        public AudioController()
        {
            _audioService = new AudioService();
        }

        [HttpPost("encode")]
        public async Task<IActionResult> Encode([FromForm] IFormFile audioFile, [FromForm] string textToEmbed)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest("Audio file is required.");

            string tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

            var encodedAudio = await _audioService.EncryptText(audioFile, textToEmbed);

            return File(encodedAudio, "audio/wav", "encoded.wav");
        }

        [HttpPost("decode")]
        public async Task<IActionResult> Decode([FromForm] IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest("Audio file is required.");

            string tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

            string extractedText = await _audioService.DecryptText(audioFile);

            System.IO.File.Delete(tempFilePath); // Clean up the temporary file

            return Ok(new { text = extractedText });
        }
    }
}
