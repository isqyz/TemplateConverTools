namespace TemplateConverTools
{
    internal class ConverWSF : ConverBase
    {
        public ConverWSF(string filePath, string? savePath = null) : base(filePath, savePath)
        {
        }
        public override bool ReadSharedStr()
        {
            for (int i = 0; i < SharedStrCount; i++)
            {
                short subLen = BigEndianReader.ReadInt16();
                var subStream = new MemoryStream(BigEndianReader.ReadBytes(subLen));
                var subRead = new BigEndianBinaryReader(subStream);
                subRead.ReadBoolean();
                var val = subRead.GetReadString();
                subRead.ReadBoolean();
                if (!int.TryParse(val, out var id) || !_sharedIDDic.TryAdd(id, subRead.GetReadString()))
                {
                    return false;
                }
            }
            return true;
        }
        public override void Special()
        {
            BigEndianReader.ReadByte();
            var key = BigEndianReader.GetReadString();
            var val = BigEndianReader.GetReadString();
        }
    }
}
