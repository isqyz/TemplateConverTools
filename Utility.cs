using Ionic.Zlib;
using System.Text;

namespace TemplateConverTools
{
    internal static class Utility
    {
        internal const string dType = "d";
        internal const string fType = "f";
        internal const string sType = "s";
        public static byte[] ComressData(byte[] originalData)
        {
            using MemoryStream originalStream = new(originalData);
            using MemoryStream compressedStream = new();
            using (ZlibStream zlibStream = new(compressedStream, Ionic.Zlib.CompressionMode.Compress))
            {
                originalStream.CopyTo(zlibStream);
            }
            return compressedStream.ToArray();
        }
        public static byte[] DecompressData(byte[] compressedData)
        {
            using MemoryStream compressedStream = new(compressedData);
            using MemoryStream decompressedStream = new();
            using (ZlibStream zlibStream = new(compressedStream, Ionic.Zlib.CompressionMode.Decompress))
            {
                zlibStream.CopyTo(decompressedStream);
            }
            return decompressedStream.ToArray();
        }
        public static bool IsEndTag(string sesName)
        {
            return "deleted" == sesName || "packEnd" == sesName;
        }
        public static void UTFConverBytes(string str, out byte[] utf8Bytes, out byte[] byteLen)
        {
            utf8Bytes = Encoding.UTF8.GetBytes(str);
            byteLen = BitConverter.GetBytes((ushort)utf8Bytes.Length);
        }
        public static void UTFConverBigEndianBytes(string str, out byte[] bytes)
        {
            bytes = Encoding.UTF8.GetBytes(str);
            var byteLen = BitConverter.GetBytes((ushort)bytes.Length).Reverse().ToArray();
            bytes = ConcatenateArrays(ref byteLen, ref bytes);

            static byte[] ConcatenateArrays(ref byte[] array1, ref byte[] array2)
            {
                byte[] result = new byte[array1.Length + array2.Length];
                Buffer.BlockCopy(array1, 0, result, 0, array1.Length);
                Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);
                return result;
            }
        }
        public static byte[] UTFConverBigEndianBytes(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var byteLen = BitConverter.GetBytes((ushort)bytes.Length).Reverse().ToArray();
            return bytes = ConcatenateArrays(ref byteLen, ref bytes);

            static byte[] ConcatenateArrays(ref byte[] array1, ref byte[] array2)
            {
                byte[] result = new byte[array1.Length + array2.Length];
                Buffer.BlockCopy(array1, 0, result, 0, array1.Length);
                Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);
                return result;
            }
        }
        public static string ByteConverString(byte[] bytes)
        {
            if (bytes.Length < 2)
                throw new ArgumentException("Byte array is too short to contain a length prefix.");

            var lengthBytes = bytes.Take(2).Reverse().ToArray();
            ushort length = BitConverter.ToUInt16(lengthBytes, 0);

            if (bytes.Length < 2 + length)
                throw new ArgumentException("Byte array does not match the length prefix.");

            var stringBytes = bytes.Skip(2).Take(length).ToArray();
            return Encoding.UTF8.GetString(stringBytes);
        }
        public static string ExtractFirstNumberString(string input)
        {
            StringBuilder numberString = new();

            foreach (char ch in input)
            {
                if (char.IsDigit(ch))
                {
                    // 如果是数字，追加到字符串中
                    numberString.Append(ch);
                }
                else
                {
                    // 如果不是数字，停止循环
                    break;
                }
            }
            return (numberString.Length > 0) ? numberString.ToString() : "0";
        }
        internal static List<string> SplitString(string input)
        {
            List<string> parts = new();

            StringBuilder part = new();
            bool insideQuotes = false;
            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                if (currentChar == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (currentChar == ',' && !insideQuotes)
                {
                    parts.Add(part.ToString().Trim());
                    part.Clear();
                }
                else
                {
                    part.Append(currentChar);
                }
            }
            parts.Add(part.ToString().Trim()); // Add the last part
            part.Clear();
            return parts;
        }
    }

}
