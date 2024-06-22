using Microsoft.AspNetCore.Http;
using SecretCommunication.BusinessLayer.Interface;
using System.Drawing;
using System.Text;

namespace SecretCommunication.BusinessLayer.Implementation
{
    public class ImageService : IImageService
    {
        public async Task<byte[]> ProcessImageAsync(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Extract LSBs (existing method)
                var lsbData = ExtractLsbData(imageBytes);

                return lsbData;
            }
        }

        public async Task<byte[]> EmbedMessageAsync(IFormFile image, string message)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Embed message into the image bytes
                var modifiedImageBytes = EmbedMessageInImage(imageBytes, message);

                return modifiedImageBytes;
            }
        }

        public async Task<string> ExtractMessageAsync(IFormFile image, int messageLength)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Extract the message from the image bytes
                var message = ExtractMessageFromImage(imageBytes, messageLength);

                return message;
            }
        }

        private byte[] ExtractLsbData(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;

                    var lsbList = new List<byte>();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color pixel = bitmap.GetPixel(x, y);

                            byte rLsb = (byte)(pixel.R & 1);
                            byte gLsb = (byte)(pixel.G & 1);
                            byte bLsb = (byte)(pixel.B & 1);

                            lsbList.Add(rLsb);
                            lsbList.Add(gLsb);
                            lsbList.Add(bLsb);
                        }
                    }

                    return lsbList.ToArray();
                }
            }
        }

        private byte[] EmbedMessageInImage(byte[] imageBytes, string message)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    int pixelCount = width * height;

                    // Convert the message to binary
                    var messageBits = GetMessageBits(message);

                    if (messageBits.Length > pixelCount * 3)
                    {
                        throw new Exception("Message is too long to be embedded in the image.");
                    }

                    int messageIndex = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color pixel = bitmap.GetPixel(x, y);

                            byte r = pixel.R;
                            byte g = pixel.G;
                            byte b = pixel.B;

                            if (messageIndex < messageBits.Length)
                            {
                                r = (byte)((r & 0xFE) | messageBits[messageIndex]);
                                messageIndex++;
                            }

                            if (messageIndex < messageBits.Length)
                            {
                                g = (byte)((g & 0xFE) | messageBits[messageIndex]);
                                messageIndex++;
                            }

                            if (messageIndex < messageBits.Length)
                            {
                                b = (byte)((b & 0xFE) | messageBits[messageIndex]);
                                messageIndex++;
                            }

                            bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                        }
                    }

                    using (var resultStream = new MemoryStream())
                    {
                        bitmap.Save(resultStream, bitmap.RawFormat);
                        return resultStream.ToArray();
                    }
                }
            }
        }

        private string ExtractMessageFromImage(byte[] imageBytes, int messageLength)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;

                    var messageBits = new List<byte>();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (messageBits.Count >= messageLength * 8)
                            {
                                break;
                            }

                            if (x < bitmap.Width && y < bitmap.Height) // Check if within bounds
                            {
                                Color pixel = bitmap.GetPixel(x, y);

                                byte rLsb = (byte)(pixel.R & 1);
                                byte gLsb = (byte)(pixel.G & 1);
                                byte bLsb = (byte)(pixel.B & 1);

                                messageBits.Add(rLsb);
                                messageBits.Add(gLsb);
                                messageBits.Add(bLsb);
                            }
                        }
                    }

                    return GetStringFromBits(messageBits.ToArray());
                }
            }
        }

        private byte[] GetMessageBits(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var messageBits = new List<byte>();

            foreach (var byteVal in messageBytes)
            {
                for (int i = 7; i >= 0; i--)
                {
                    messageBits.Add((byte)((byteVal >> i) & 1));
                }
            }

            return messageBits.ToArray();
        }

        private string GetStringFromBits(byte[] bits)
        {
            var messageBytes = new List<byte>();

            for (int i = 0; i < bits.Length; i += 8)
            {
                byte byteVal = 0;

                for (int j = 0; j < 8; j++)
                {
                    byteVal = (byte)((byteVal << 1) | bits[i + j]);
                }

                messageBytes.Add(byteVal);
            }

            return Encoding.UTF8.GetString(messageBytes.ToArray());
        }
    }
}
