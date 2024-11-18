using System.IO;

namespace ArmsFW.Services.Logging
{
    public class FileLog
    {
        public string FileName { get; set; }
        public FileLog FileLogOrigem { get; set; }
        public string Diretorio { get; set; }

        internal int GetCountLines()
        {
            try
            {
                if (!File.Exists(FileName)) return 0;
                return File.ReadAllLines(FileName).Length;
            }
            catch
            {
            }

            return 0;
        }

        internal bool Existe()
        {
            return File.Exists(FileName);
        }

        internal long Tamanho()
        {
            return new FileInfo(FileName).Length;
        }
    }
}
