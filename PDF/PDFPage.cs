using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Services.PDF
{
    public class PDFPage
    {

        private readonly string _text;
        private readonly int _pageNumber;

        public PDFPage()
        {
            Lines = new Dictionary<int, string>();
        }
        public PDFPage(string content, int pageNumber) : this()
        {
            _text = content;
            _pageNumber = pageNumber;
            _text.Split("\n".ToCharArray()).ToList().ForEach(linha => Lines.Add(Lines.Count, linha.ToString()));
        }

        public string TextContent { get => _text; }
        public int PageNumber { get => _pageNumber; }

        public Dictionary<int, string> Lines { get; set; } = new Dictionary<int, string>();
    }
}
