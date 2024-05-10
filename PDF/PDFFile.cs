using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Services.PDF
{
    public class PDFFile
    {
        public PDFFile()
        {

        }
        public PDFFile(PDFDoc doc)
        {
            FilePath = doc.FileSource;
            Docs.Add(doc);
        }
        /// <summary>
        /// Arquivo de origem 
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// Recupera todo o conteudo Texto de todo o documento
        /// </summary>
        public string TextContent
        {
            get
            {
                string returns = "";
                Docs.ForEach(d => returns+= d.TextContent);
                return returns;
            }
        }
        /// <summary>
        /// Recupera o conteudo Binario do documento
        /// </summary>
        public byte[] Content
        {
            get
            {
                byte[] returns = new byte[0];
                Docs.ForEach(d => returns.Concat(d.BytesContent.ToArray()));
                return returns;
            }
        }
        /// <summary>
        /// Relação dos documentos de origem
        /// </summary>
        public List<PDFDoc> Docs { get; set; } = new List<PDFDoc>();
        //Lista de Paginas
        public List<PDFPage> Pages { get {
                var pgs = new List<PDFPage>();
                Docs.ForEach(d => pgs.AddRange(d.Pages.ToList().Select(p => p.Value)));
                return pgs;
            }
        }

        public LoadResult StatusLoadResult { get; set; } = new LoadResult();
    }

    public class LoadResult
    {
        public LoadResult()
        {

        }
        public LoadResult(bool success) : this() => Success = success;

        public LoadResult(bool success, string messagem) : this(success) => Message = messagem;

        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Exception> Exceptions { get; set; } = new List<Exception>();
    }
}
