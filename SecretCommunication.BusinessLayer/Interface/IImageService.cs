namespace SecretCommunication.BusinessLayer.Interface
{
    public interface IImageService
    {
        Task EncodeMessageIntoImage();
        Task DecodeMessageFromImage();
    }
}
