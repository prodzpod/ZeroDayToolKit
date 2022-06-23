using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;

namespace ZeroDayToolKit.Utils
{
    public class MathUtils
    {
        public static float Ratio(float a1, float b1, float a2, float b2, float a3)
        {
            return b1 + ((a3 - a1) * (b1 - b2) / (a1 - a2));
        }

        public static string encodeBase64(string txt)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(txt));
        }

        public static string decodeBase64(string txt)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(txt));
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static string encodeBinary(string txt)
        {
            return string.Join("", txt.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        public static string encodeBinary(byte[] txt)
        {
            return string.Join("", txt.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        public static string decodeBinary(string txt)
        {
            if ((txt.Length % 8) != 0) return null;
            string ret = "";
            try
            {
                for (int i = 0; i < txt.Length; i += 8) ret += Convert.ToChar(Convert.ToByte(txt.Substring(i, 8), 2));
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string encodeZip(string txt)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(txt);
            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    gZipStream.Write(inputBytes, 0, inputBytes.Length);
                return encodeBinary(memoryStream.ToArray());
            }
        }

        public static string decodeZip(string txt)
        {
            if ((txt.Length % 8) != 0) return null;
            List<byte> binary = new List<byte>();
            try
            {
                for (int i = 0; i < txt.Length; i += 8) binary.Add(Convert.ToByte(txt.Substring(i, 8), 2));
                using (var memoryStream = new MemoryStream(binary.ToArray()))
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                using (var streamReader = new StreamReader(gZipStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
