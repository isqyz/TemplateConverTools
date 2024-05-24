namespace TemplateConverTools
{
    internal class ConverSSR : ConverBase
    {
        public ConverSSR(string filePath, string? savePath = null) : base(filePath, savePath)
        {
            BigEndianReader.Varint32 = true;
        }
    }
}
