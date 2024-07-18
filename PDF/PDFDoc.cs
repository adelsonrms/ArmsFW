//using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArmsFW.Services.PDF
{
    /// <summary>
    /// Abstrai as informações genericas de um arquivo PDF
    /// </summary>
    public class PDFDoc
    {
        private DocInfo _pdfDoc;

        public DocInfo PDFReader => _pdfDoc;

        public PDFDoc(DocInfo doc, bool bLoadContent)
        {
            _pdfDoc = doc;
            this.FileSource = doc.FileSource ?? doc.FileName;
            this.FileName = doc.FileName;
            this.FolderPath = Path.GetDirectoryName(doc.FileName);
            
            if (bLoadContent)
            {
                this.BytesContent = File.ReadAllBytes(doc.FileName);
                LoadFromBytes(this.BytesContent, true);
            }
            else
            {
                if (this.FileName.ToUpper().EndsWith(".TXT"))
                {
                    
                    this.LoadTextFromCacheFile();
                }
            }
        }

        public PDFDoc(string ArquivoPDF, bool bLoadContent)
        {
            this.FileSource = ArquivoPDF;
            this.BytesContent = File.ReadAllBytes(ArquivoPDF);
            this.FileName = System.IO.Path.GetFileName(ArquivoPDF);
            this.FolderPath = Directory.GetParent(ArquivoPDF).FullName;

            LoadFromBytes(this.BytesContent, bLoadContent);
        }

        public PDFDoc(byte[] arrBytes) => LoadFromBytes(arrBytes, true);

        public PDFDoc(byte[] arrBytes, bool bLoadContent) => LoadFromBytes(arrBytes, bLoadContent);

        public PDFDoc(Stream StArquivoPDF)
        {
            LoadFromBytes(new BinaryReader(StArquivoPDF).ReadBytes((int)StArquivoPDF.Length), true);
        }

        private void LoadFromBytes(byte[] arrBytes, bool bGetText)
        {
            try
            {
                BytesContent = arrBytes;
                this.Pages = new Dictionary<int, PDFPage>();

                using (this.StreamContent = new MemoryStream(this.BytesContent)) { }
                
                if (bGetText) ExtractTextFromPDF();

                Valid = true;
                StatusLoaded = $"Documento PDF valida e carregado com sucesso. Detalhe : Qtd Paginas {_pdfDoc.GetNumberOfPages()}";
            }
            catch (Exception ex)
            {
                Valid = false;
                StatusLoaded = $"Erro na leitura do documento PDF. Detalhe : {ex.Message}";
            }
        }

        private string _text;
        public string TextContent { get => _text; }
        public Stream StreamContent { get; set; }
        public byte[] BytesContent { get; set; }

        public string FileSource { get; set; }
        public string FileName { get; set; }
        public string FolderPath { get; set; }
        public string FullName { get => string.Format("{0}\\{1}", this.FolderPath, this.FileName); }

        public string sourceType { get; set; }
        
        public Dictionary<int, PDFPage> Pages { get; set; }
        public PDFPage FirstPage => this?.Pages[1] ?? new PDFPage();

        public bool Valid { get; private set; }
        public string StatusLoaded { get; private set; }

        public string LoadTextFromCacheFile()
        {
            try
            {
                this.sourceType = "Cache";
                Valid = true;
                StatusLoaded = $"Conteudo Texto Carregado !";
                this.Pages = new Dictionary<int, PDFPage>();
                _text = File.ReadAllText(this.FileName);
                this.Pages.Add(1, new PDFPage(_text, 1));
            }
            catch 
            {
            }
            return _text;
        }

        private string ExtractTextFromPDF()
        {
            try
            {
                this.sourceType = "PDF";
                this.Pages = new Dictionary<int, PDFPage>();
                _text = "";

                for (var indexPagina = 1; indexPagina <= _pdfDoc.GetNumberOfPages(); indexPagina++)
                {
                    //Recupera o texto do documento utilizado a biblioteca BitMiracle.Docotic.
                    //Com essa lib é possivel extrair o texto formatado. No iText nao conseguimos recueperar o texto formatado
                    string _textPage = PDFDocoticService.GetTextWithDocotic(this.FileName, indexPagina);

                    ////var _textPage = iTextSharpService.ExtractTextFromPDF(this.FileName);

                    //using (BitMiracle.Docotic.Pdf.PdfDocument pdf = new BitMiracle.Docotic.Pdf.PdfDocument(this.FileName))
                    //{
                    //    _textPage = pdf.GetTextWithFormatting();//(opcoes);
                    //}

                    _text += _textPage;
                    this.Pages.Add(indexPagina, new PDFPage(_textPage, indexPagina));

                    File.WriteAllTextAsync(this.FileName.Replace(".PDF", ".TXT"), _textPage);
                }

                try
                {
                    //File.Copy(this.FileSource, @$"{this.FolderPath}\{Path.GetFileName(this.FileSource)}", true);
                    //File.WriteAllTextAsync(@$"{this.FolderPath}\{Path.GetFileNameWithoutExtension(this.FileSource)}.TXT", _text);
                }
                catch 
                {
                }

                return _text;
            }
            catch (Exception pdfEx)
            {
                return "Error : " + pdfEx.Message;
            }
        }

        public string getMetaTag(string key)
        {
            try
            {
                return "";//this.PDFReader.GetDocumentInfo().GetMoreInfo(key);
            }
            catch
            {
                return string.Empty;
            }
        }

        public KeyValuePair<int, string> LocalizaLinhaPorTexto(string texto)
        {
            KeyValuePair<int, string> linha = new KeyValuePair<int, string>();
            var linhas = this.FirstPage.Lines.Where(item => item.Value.Contains(texto));
            if (linhas.Count() > 0)
            {
                linha = linhas.First();
            }
            return linha;
        }
    }

    public interface IPDFService
    {
        PDFDoc Load();
        string GetTextFromPage(int PageNumber);
    }
}
