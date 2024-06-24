using Microsoft.AspNetCore.Http;
using SecretCommunication.BusinessLayer.Interface;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

public class ImageService : IImageService
{
    public async Task<byte[]> EmbedMessageAsync(IFormFile image, string message)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("No image uploaded.");
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("No message provided.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await image.CopyToAsync(memoryStream);
            using (var sourceImage = new Bitmap(memoryStream))
            {
                int messageLength = message.Length;
                int imageWidth = sourceImage.Width, imageHeight = sourceImage.Height, imageSize = imageWidth * imageHeight;

                if (messageLength * 8 + 32 > imageSize)
                {
                    throw new ArgumentException("Message is too long for the chosen image");
                }

                Bitmap embeddedImage = new Bitmap(sourceImage);
                EmbedInteger(embeddedImage, messageLength, 0, 0);

                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                for (int i = 0; i < messageBytes.Length; i++)
                {
                    EmbedByte(embeddedImage, messageBytes[i], i * 8 + 32, 0);
                }

                using (var resultStream = new MemoryStream())
                {
                    ImageFormat format = GetImageFormat(image.FileName);
                    embeddedImage.Save(resultStream, format);
                    return resultStream.ToArray();
                }
            }
        }
    }

    public async Task<string> DecodeMessageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("No image uploaded.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await image.CopyToAsync(memoryStream);
            using (var sourceImage = new Bitmap(memoryStream))
            {
                int messageLength = ExtractInteger(sourceImage, 0, 0);
                byte[] messageBytes = new byte[messageLength];
                for (int i = 0; i < messageLength; i++)
                {
                    messageBytes[i] = ExtractByte(sourceImage, i * 8 + 32, 0);
                }
                return Encoding.ASCII.GetString(messageBytes);
            }
        }
    }

    private int ExtractInteger(Bitmap img, int start, int storageBit)
    {
        int maxX = img.Width, maxY = img.Height;
        int startX = start / maxY, startY = start % maxY, count = 0;
        int length = 0;
        for (int i = startX; i < maxX && count < 32; i++)
        {
            for (int j = startY; j < maxY && count < 32; j++)
            {
                Color pixel = img.GetPixel(i, j);
                int rgb = pixel.ToArgb();
                int bit = GetBitValue(rgb, storageBit);
                length = SetBitValue(length, count, bit);
                count++;
            }
        }
        return length;
    }

    private byte ExtractByte(Bitmap img, int start, int storageBit)
    {
        int maxX = img.Width, maxY = img.Height;
        int startX = start / maxY, startY = start % maxY, count = 0;
        byte b = 0;
        for (int i = startX; i < maxX && count < 8; i++)
        {
            for (int j = startY; j < maxY && count < 8; j++)
            {
                Color pixel = img.GetPixel(i, j);
                int rgb = pixel.ToArgb();
                int bit = GetBitValue(rgb, storageBit);
                b = (byte)SetBitValue(b, count, bit);
                count++;
            }
        }
        return b;
    }

    private void EmbedInteger(Bitmap img, int n, int start, int storageBit)
    {
        int maxX = img.Width, maxY = img.Height, startX = start / maxY, startY = start % maxY, count = 0;
        for (int i = startX; i < maxX && count < 32; i++)
        {
            for (int j = startY; j < maxY && count < 32; j++)
            {
                Color pixel = img.GetPixel(i, j);
                int rgb = pixel.ToArgb();
                int bit = GetBitValue(n, count);
                rgb = SetBitValue(rgb, storageBit, bit);
                img.SetPixel(i, j, Color.FromArgb(rgb));
                count++;
            }
        }
    }

    private void EmbedByte(Bitmap img, byte b, int start, int storageBit)
    {
        int maxX = img.Width, maxY = img.Height, startX = start / maxY, startY = start % maxY, count = 0;
        for (int i = startX; i < maxX && count < 8; i++)
        {
            for (int j = startY; j < maxY && count < 8; j++)
            {
                Color pixel = img.GetPixel(i, j);
                int rgb = pixel.ToArgb();
                int bit = GetBitValue(b, count);
                rgb = SetBitValue(rgb, storageBit, bit);
                img.SetPixel(i, j, Color.FromArgb(rgb));
                count++;
            }
        }
    }

    private int GetBitValue(int n, int location)
    {
        int v = n & (1 << location);
        return v == 0 ? 0 : 1;
    }

    private int SetBitValue(int n, int location, int bit)
    {
        int toggle = 1 << location;
        int bv = GetBitValue(n, location);
        if (bv == bit)
            return n;
        if (bv == 0 && bit == 1)
            n |= toggle;
        else if (bv == 1 && bit == 0)
            n ^= toggle;
        return n;
    }

    private ImageFormat GetImageFormat(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        switch (extension)
        {
            case ".bmp":
                return ImageFormat.Bmp;
            case ".jpg":
            case ".jpeg":
                return ImageFormat.Jpeg;
            case ".png":
                return ImageFormat.Png;
            case ".gif":
                return ImageFormat.Gif;
            case ".tiff":
                return ImageFormat.Tiff;
            default:
                return ImageFormat.Png;
        }
    }
}
