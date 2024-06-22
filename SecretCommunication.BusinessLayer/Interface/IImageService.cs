using Microsoft.AspNetCore.Http;

namespace SecretCommunication.BusinessLayer.Interface
{
    public interface IImageService
    {
        Task<byte[]> ProcessImageAsync(IFormFile image);
        Task<byte[]> EmbedMessageAsync(IFormFile image, string message);
        Task<string> ExtractMessageAsync(IFormFile image, int messageLength);
    }
}
