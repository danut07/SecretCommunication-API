using NAudio.Wave;
using System.Text;

namespace SecretCommunication_API.BusinessLayer
{
    public class AudioService
    {
        public void EmbedData(string audioFilePath, string outputFilePath, string data)
        {
            byte[] binaryData = Encoding.UTF8.GetBytes(data);
            int dataLen = binaryData.Length * 8; // Each byte is 8 bits

            using (var reader = new AudioFileReader(audioFilePath))
            {
                WaveFileWriter.CreateWaveFile16(outputFilePath, reader);
            }

            using (var waveFile = new WaveFileReader(outputFilePath))
            {
                byte[] bytes = new byte[waveFile.Length];
                waveFile.Read(bytes, 0, bytes.Length);

                // Embed data in the least significant bit of each byte
                int dataIndex = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (dataIndex < dataLen)
                    {
                        bytes[i] = (byte)((bytes[i] & 0xFE) | ((binaryData[dataIndex / 8] >> (dataIndex % 8)) & 1));
                        dataIndex++;
                    }
                }

                File.WriteAllBytes(outputFilePath, bytes);
            }
        }

        public string ExtractData(string stegoFilePath, int dataLen)
        {
            using (var waveFile = new WaveFileReader(stegoFilePath))
            {
                byte[] bytes = new byte[waveFile.Length];
                waveFile.Read(bytes, 0, bytes.Length);

                var binaryData = new StringBuilder(dataLen * 8);

                // Extract data from the least significant bit of each byte
                for (int i = 0; i < dataLen * 8; i++)
                {
                    binaryData.Append((bytes[i] & 1).ToString());
                }

                // Convert binary string to text
                var dataBytes = Enumerable.Range(0, binaryData.Length / 8)
                                          .Select(i => Convert.ToByte(binaryData.ToString().Substring(i * 8, 8), 2))
                                          .ToArray();

                return Encoding.UTF8.GetString(dataBytes);
            }
        }
    }
}
