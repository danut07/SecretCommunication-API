using Microsoft.AspNetCore.Http;

namespace SecretCommunication.BusinessLayer.Interface
{

    public interface IAudioService
    {
        Task<byte[]> EncryptText(IFormFile wav, string text);
        byte[] EncryptTextLinear(byte[] wav, string text);
        Task<string> DecryptText(IFormFile wav);
        string DecryptTextLinear(byte[] wav);
    }
}
