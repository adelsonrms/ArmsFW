
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System;
using System.Collections.Generic;

namespace ArmsFW.Services.PDF
{
    public partial class iTextService
    {
        /// <summary>
        /// Essa classe é utilizada para fazer a quebra do PDF em paginas em arquivos distintos por quantidade de paginas
        /// </summary>
        private class PageCountSplitter : PdfSplitter
        {
            int _partNumber = 0;
            private string _destino;
            private PdfDocument _doc;
            /// <summary>
            /// Guarda a lista dos arquivos que são gerados
            /// </summary>
            public Dictionary<int, string> Docs { get; set; }
            /// <summary>
            /// Inicializa o spliter (quebra)
            /// </summary>
            /// <param name="pdfDocument">Instancia do PDF Documento</param>
            /// <param name="pathDestino">Pasta de destino</param>
            public PageCountSplitter(PdfDocument pdfDocument, string pathDestino) : base(pdfDocument)
            {
                Docs = new Dictionary<int, string>();
                _doc = pdfDocument;
                _destino = pathDestino;
            }
            /// <summary>
            /// Invoca a leitura do documento do PDF e gera o arquivo de saida
            /// </summary>
            /// <param name="documentPageRange"></param>
            /// <returns></returns>
            protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
            {
                try
                {
                    _partNumber++;
                    Docs.Add(_partNumber, $"{_destino}\\{_partNumber.ToString("000")}.PDF");

                    var w = new PdfWriter(Docs[_partNumber]);

                    return w;

                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}
