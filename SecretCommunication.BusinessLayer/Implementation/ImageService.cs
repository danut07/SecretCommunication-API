using SecretCommunication.BusinessLayer.Interface;
using System.Drawing;

namespace SecretCommunication.BusinessLayer.Implementation
{
    public class ImageService : IImageService
    {
        public Task EncodeMessageIntoImage(string filename)
        {
            Bitmap img = new Bitmap(filename);
        }

        public Task DecodeMessageFromImage()
        {
            return Task.CompletedTask;
        }
    }
}
