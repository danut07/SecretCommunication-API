using Microsoft.AspNetCore.Http;

namespace SecretCommunication.BusinessLayer.Interface
{
    public interface IImageService
    {
        Task<byte[]> EmbedMessageAsync(IFormFile image, string message);
        Task<string> DecodeMessageAsync(IFormFile image);
    }
}
