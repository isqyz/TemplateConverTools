using System.Text;

namespace TemplateConverTools
{
    internal class ConverBase : IDisposable
    {
        public readonly BigEndianBinaryReader BigEndianReader;
        public readonly Dictionary<int, string> _sharedIDDic = new();
        private readonly MemoryStream data;
        private readonly List<string> fileNameList = new();
        private readonly UTF8Encoding utf8WithBom = new(true);
        private readonly string savePath = "./csvTemp";
        private const string tabName = "mydb_sharedStr_internaltbl";
        private StreamWriter? writer;
        private FileStream? fs;
        public Action<string> onError;
        public int SharedStrCount = 0;
        public ConverBase(string filePath,string? savePath = null)
        {
            var templateFile = new FileInfo(filePath);
            if (!templateFile.Exists)
            {
                throw new FileNotFoundException(filePath);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            data = new MemoryStream(Utility.DecompressData(File.ReadAllBytes(templateFile.FullName)));
            BigEndianReader = new BigEndianBinaryReader(data);
            onError += Console.WriteLine;
            if(savePath is not null)
            {
                this.savePath = savePath;
            }
            Directory.CreateDirectory(this.savePath);
        }
        public bool ReadFile()
        {
            var bytes = Utility.UTFConverBigEndianBytes(tabName);
            BigEndianReader.ReadBytes(bytes.Length);
            ReadSharedStringSize();
            if (!ReadSharedStr())
            {
                return false;
            }
            while (true)
            {
                ReadFileName();
                ReadTableSize(out int col, out byte row);
                WriteTable(ref row, ref col);
                writer?.Close();
                writer?.Dispose();
                fs?.Close();
                fs?.Dispose();
                if (data.Position == data.Length)
                {
                    break;
                }
            }
            return true;
        }
        private bool ReadSharedStringSize()
        {
            SharedStrCount = BigEndianReader.ReadInt32();
            Special();
            return true;
        }
        public virtual bool ReadSharedStr()
        {
            return true;
        }
        public virtual void Special() 
        {

        }
        private void ReadFileName()
        {
            fileNameList.Add(BigEndianReader.GetReadString());
            fs = new FileStream($"{savePath}/{fileNameList[^1]}.csv", FileMode.Create, FileAccess.Write);
            writer = new StreamWriter(fs, utf8WithBom);
        }
        private void ReadTableSize(out int col, out byte row)
        {
            col = BigEndianReader.ReadInt32();
            row = BigEndianReader.ReadByte();
        }
        private void WriteTable(ref byte row, ref int col)
        {
            if (writer is null)
            {
                return;
            }
            string[] rowNameS = new string[row];
            //write row
            for (int i = 0; i < row; rowNameS[i] = BigEndianReader.GetReadString(), writer.Write(rowNameS[i], i++))
            {
                if (i > 0)
                {
                    writer.Write(',');
                }
            }
            //write col
            for (int i = 0; i < col; i++)
            {
                writer.Write('\n');
                var subLen = BigEndianReader.ReadInt16();
                var endLen = data.Position + subLen;
                for (int j = 0; j < rowNameS.Length; j++) 
                {
                    var spl = rowNameS[j].Split('|');
                    if (spl.Length>=2)
                    {
                        switch(spl[1])
                        {
                            case Utility.fType:
                                {
                                    writer.Write(BigEndianReader.ReadFloat());
                                    break;
                                }
                            case Utility.dType:
                                {
                                    writer.Write(WriteDecimal());
                                    break;
                                }
                            case Utility.sType:
                            default:
                                {
                                    writer.Write(WriteString());
                                    break;
                                }
                        }
                    }
                    else
                    {
                        writer.Write(WriteString());
                    }
                    if (data.Position >= endLen)
                    {
                        break;
                    }
                    writer.Write(',');
                }
            }
        }
        public virtual string WriteString()
        {
            if (BigEndianReader.ReadBoolean())
            {
                return _sharedIDDic[BigEndianReader.ReadInt32()];
            }
            else
            {
                return BigEndianReader.GetReadString();
            }
        }
        public virtual string WriteDecimal()
        {
            return BigEndianReader.ReadInt32().ToString();
        }
        public void Dispose()
        {
            writer?.Close();
            writer?.Dispose();
            fs?.Close();
            fs?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
