
using ArmsFW.Services.Extensions;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArmsFW.Services.PDF
{
    public partial class iTextService
    {
        public static List<PDFDoc> GetPDFDocFromPages(dynamic source, string tempPasta = "")
        {
            //Inicializa as variáveis do iTextSharp utilizada
            PdfReader reader = null;
            PdfDocument pdfDocument = null;
            Stream fs = null;

            List<PDFDoc> listDocs = new List<PDFDoc>();

            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_hhmmss");

            if (string.IsNullOrEmpty(tempPasta))
            {
                tempPasta = $@"{Environment.GetEnvironmentVariable("temp")}\_pdf";
            }
            
            Directory.CreateDirectory(tempPasta);

            try
            {
                //Carrega o stream do documento. Seja de um arquivo fisico, seja de um array de bytes
                fs = ((!((source.GetType().Name == "Byte[]") ? true : false)) ? new FileStream(source, FileMode.Open) : (source as byte[]).GetStream());
                
                //Carrega o reader do pdf
                reader = new PdfReader(fs);
                
                int maxPageCount = 1; 

                pdfDocument = new PdfDocument(reader);

                var spliter = new PageCountSplitter(pdfDocument, tempPasta);

                IList<PdfDocument> splitDocuments = spliter.SplitByPageCount(maxPageCount);
                
                int part = 1;
                foreach (PdfDocument doc in splitDocuments)
                {
                    var pg = doc.GetFirstPage();
                    var docInfo = new DocInfo(filename: spliter.Docs[part], PageNumber: doc.GetNumberOfPages());

                    if (source.GetType().Name == "String") docInfo.FileSource = source;

                    doc.Close();

                    listDocs.Add(new PDFDoc(docInfo, true));

                    part++;
                }

                fecharDocumento();
            }
            catch (Exception ex)
            {
                fecharDocumento();
                throw new Exception(ex.Message, ex);
            }

            return listDocs;


            void fecharDocumento()
            {
                pdfDocument?.Close();
                fs?.Close();
                reader?.Close();

            }
        }
    }
}
