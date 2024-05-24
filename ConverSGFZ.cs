namespace TemplateConverTools
{
    internal class ConverSGFZ : ConverBase
    {
        public ConverSGFZ(string filePath, string? savePath = null) : base(filePath, savePath)
        {
        }

        public override bool ReadSharedStr()
        {
            for (int i = 0; i < SharedStrCount; i++)
            {
                if (SharedStrCount!=((int)BigEndianReader.BaseStream.Position-32))
                {
                    if (!_sharedIDDic.TryAdd((int)BigEndianReader.BaseStream.Position, BigEndianReader.GetReadString()))
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        public override string WriteDecimal()
        {
            switch(BigEndianReader.ReadByte())
            {
                case 1:
                case 11:
                    {
                        return BigEndianReader.ReadByte().ToString();
                    }
                case 2:
                case 12:
                    {
                        return BigEndianReader.ReadInt16().ToString();
                    }
                case 4:
                    {
                        return BigEndianReader.ReadInt32().ToString();
                    }
                case 8:
                case 18:
                    {
                        return BigEndianReader.ReadInt64().ToString();
                    }
            }
            return "0";
        }
    }
}
