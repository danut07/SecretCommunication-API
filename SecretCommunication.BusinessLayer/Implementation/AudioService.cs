using Microsoft.AspNetCore.Http;
using SecretCommunication.BusinessLayer.Interface;
using SecretCommunication_API.Models;
using SecretCommunication_API.Models.AudioSteganography;

namespace SecretCommunication_API.BusinessLayer
{
    public class AudioService : IAudioService
    {
        public async Task<byte[]> EncryptText(IFormFile wav, string text)
        {
            using(var memoryStream = new MemoryStream())
            {
                await wav.CopyToAsync(memoryStream);

                WavAudio audio = new WavAudio(memoryStream.ToArray());
                uint value = 0;
                string pass = string.Format(audio.bitsPerSample.ToString());
                AESEncrypt encrypt = new AESEncrypt();
                string encrypted = encrypt.EncryptString(text, pass);
                if (encrypted.Length <= Math.Floor((double)(audio.totalSamples / 8)))
                {
                    SeedURNG generator = new SeedURNG(audio.totalSamples, audio.totalSamples);
                    for (int i = 0; i < encrypted.Length; i++)
                    {
                        value = encrypted[i];
                        for (int x = 0; x < 8; x++)
                        {
                            uint sample = generator.Next;
                            uint sampleValue = audio.samples[sample];
                            sampleValue = (sampleValue & 0xFFFFFFFE) | ((value >> x) & 1);
                            audio.samples[sample] = sampleValue;
                        }

                    }
                    value = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        uint sample = generator.Next;
                        uint sampleValue = audio.samples[sample];
                        sampleValue = (sampleValue & 0xFFFFFFFE) | ((value >> x) & 1);
                        audio.samples[sample] = sampleValue;
                    }
                    audio.Save();
                    return audio.data;
                }
                else
                {
                    return null;
                }
            }
        }

        public  byte[] EncryptTextLinear(byte[] wav, string text)
        {
            WavAudio audio = new WavAudio(wav);
            uint value = 0;
            string pass = string.Format(audio.bitsPerSample.ToString());
            AESEncrypt encrypt = new AESEncrypt();
            string encrypted = encrypt.EncryptString(text, pass);
            if (encrypted.Length <= Math.Floor((double)(audio.totalSamples / 8)))
            {
                uint n = 0;
                for (int i = 0; i < encrypted.Length; i++)
                {
                    value = encrypted[i];
                    for (int x = 0; x < 8; x++)
                    {
                        uint sample = n;
                        uint sampleValue = audio.samples[sample];
                        sampleValue = (sampleValue & 0xFFFFFFFE) | ((value >> x) & 1);
                        audio.samples[sample] = sampleValue;
                        n++;
                    }

                }
                value = 0;
                for (int x = 0; x < 8; x++)
                {
                    uint sample = n;
                    uint sampleValue = audio.samples[sample];
                    sampleValue = (sampleValue & 0xFFFFFFFE) | ((value >> x) & 1);
                    audio.samples[sample] = sampleValue;
                    n++;
                }
                audio.Save();
                return audio.data;
            }
            else
            {
                return null;
            }

        }

        public async Task<string> DecryptText(IFormFile wav)
        {
            using(var memoryStream = new MemoryStream())
            {
                await wav.CopyToAsync(memoryStream);

                WavAudio audio = new WavAudio(memoryStream.ToArray());
                string text = string.Empty;
                SeedURNG generator = new SeedURNG(audio.totalSamples, audio.totalSamples);
                uint value = 0;
                string pass = string.Format(audio.bitsPerSample.ToString());
                AESEncrypt encrypt = new AESEncrypt();
                do
                {
                    value = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        uint sample = generator.Next;
                        uint sampleValue = audio.samples[sample];
                        value = value | ((sampleValue & 1) << x);
                    }
                    if (value != 0)
                        text += Convert.ToChar(value);
                } while (value != 0);
                try
                {
                    return encrypt.DecryptString(text, pass);
                }
                catch (Exception e)
                {
                    return e.StackTrace;
                }
            }        
        }

        public string DecryptTextLinear(byte[] wav)
        {
            WavAudio audio = new WavAudio(wav);
            string text = string.Empty;
            uint n = 0;
            uint value = 0;
            string pass = string.Format(audio.bitsPerSample.ToString());
            AESEncrypt encrypt = new AESEncrypt();
            do
            {
                value = 0;
                for (int x = 0; x < 8; x++)
                {
                    uint sample = n;
                    uint sampleValue = audio.samples[sample];
                    value = value | ((sampleValue & 1) << x);
                    n++;
                }
                if (value != 0)
                    text += Convert.ToChar(value);
            } while (value != 0);
            try
            {
                return encrypt.DecryptString(text, pass);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
