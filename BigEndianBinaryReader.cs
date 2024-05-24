using System.Text;

namespace TemplateConverTools
{
    internal class BigEndianBinaryReader : BinaryReader
    {
        public bool Varint32 = false;
        public BigEndianBinaryReader(Stream input) : base(input)
        {
        }

        public BigEndianBinaryReader(Stream input, System.Text.Encoding encoding) : base(input, encoding)
        {
        }

        public BigEndianBinaryReader(Stream input, System.Text.Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }
        public override short ReadInt16()
        {
            return BitConverter.ToInt16(base.ReadBytes(2).Reverse().ToArray());
        }
        public override int ReadInt32()
        {
            if(Varint32)
            {
                return ReadRawVarint32();
            }
            return BitConverter.ToInt32(base.ReadBytes(4).Reverse().ToArray());
        }
        public float ReadFloat()
        {
            return BitConverter.Int32BitsToSingle(BitConverter.ToInt32(base.ReadBytes(4).Reverse().ToArray()));
        }
        public override long ReadInt64()
        {
            return BitConverter.ToInt64(base.ReadBytes(8).Reverse().ToArray());
        }

        public string GetReadString()
        {
            var len = ReadInt16();
            return Encoding.UTF8.GetString((base.ReadBytes(len).ToArray()));
        }

        public int ReadRawVarint32()
        {
            uint result = 0;
            int shift = 0;

            while (true)
            {
                byte b = ReadByte();
                result |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    break;
                }
                shift += 7;
            }
            //DecodeZigZag32
            return (int)(result >> 1) ^ -((int)(result & 1));
        }
    }
}
