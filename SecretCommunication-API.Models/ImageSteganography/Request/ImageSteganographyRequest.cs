using Microsoft.AspNetCore.Http;

namespace SecretCommunication_API.Models.ImageSteganography.Request
{
    public class ImageSteganographyRequest
    {
        public IFormFile Image { get; set; }
        public string Message { get; set; } = default!;
    }
}
