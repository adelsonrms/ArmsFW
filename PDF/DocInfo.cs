using iText.Kernel.Pdf;

namespace ArmsFW.Services.PDF
{
    public class DocInfo
    {
        public DocInfo(string filename, int PageNumber)
        {
            this.FileName = filename;
            this.PageNumber = PageNumber;
        }

        public int PageNumber { get; set; }
        public string PageVersion { get; set; }
        public string FileName { get; set; }
        public string FileSource { get; set; }

        public byte[] Content { get; set; }

        public int GetNumberOfPages() => PageNumber;
        public string GetPdfVersion() => PageVersion;
    }
}
