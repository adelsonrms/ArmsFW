
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArmsFW.Services.PDF
{
    public sealed class iTextSharpService
    {
        public static string errormsg;
        public static string PastaTemporaria { get; set; }
        public static string ArquivoTXTTemporario { get; set; }

        public static string[] SavePDFToText(string strFileInput, string strOutputFile = "", bool bDeleteTempFile = true)
        {

            string strText = null;
            string[] Lines = null;
            string sTempPath = "";
            string strTempTXT = null;

            try
            {

                strText = string.Empty;

                if (!string.IsNullOrWhiteSpace(strOutputFile))
                    sTempPath = strOutputFile;
                else
                {
                    sTempPath = string.Format("{0}\\PDFSplit", Environment.GetEnvironmentVariable("temp"));
                    if (!Directory.Exists(sTempPath)) { Directory.CreateDirectory(sTempPath); };
                    sTempPath = string.Format("{0}\\{1:yyyyMMdd_hhmmss}", sTempPath, DateTime.Now);
                    if (!Directory.Exists(sTempPath)) { Directory.CreateDirectory(sTempPath); };
                }

                PastaTemporaria = sTempPath;
                ArquivoTXTTemporario = strTempTXT;

                strTempTXT = sTempPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(strFileInput) + ".TXT";
                var pdf = new PDFDoc(strFileInput, false);

                var ListaDeArquivos = iTextSharpService.SplitPDF(strFileInput, sTempPath);

                foreach (string arquivoPDF in ListaDeArquivos)
                {
                   // pdf.ConverterParaText(arquivoPDF);
                }

                //pdf.SalvarArquivoTexto(strTempTXT);

                if (bDeleteTempFile)
                {
                    if (Directory.Exists(sTempPath))
                        Directory.Delete(sTempPath, true);
                }
                Lines = pdf.TextContent.Split("\n".ToCharArray());

            }
            catch
            {

            }
            return Lines;
        }

        /// <summary>
        /// Extrai o texto usando o iTextSharp
        /// </summary>
        /// <param name="pPDFFile"></param>
        /// <returns></returns>
        public static string ExtractTextFromPDF(string pPDFFile)
        {
            dynamic sOut = "";
            using (PdfReader _pdfDoc = new PdfReader(pPDFFile))
            {
                for (var i = 1; i <= _pdfDoc.NumberOfPages; i++)
                {
                    iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy its = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    sOut += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(_pdfDoc, i, its) + "\n";
                }
            }
            return sOut;
        }

        public static void PDFTxtToPdf(string sTxtfile, string sPDFSourcefile)
        {
            StreamReader sr = new StreamReader(sTxtfile);
            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(sPDFSourcefile, FileMode.Create));
            doc.Open();
            doc.Add(new Paragraph(sr.ReadToEnd()));
            doc.Close();
        }

        /// <summary>
        /// Quebra o PDF em paginas utlizando a biblioteca iTextSharp
        /// </summary>
        /// <param name="pFileSource">Arquivo de entrada</param>
        /// <param name="DestinationPath">Local de destino</param>
        public static string[] SplitPDF(dynamic pFileSource, string DestinationPath = "")
        {
            string[] vArquivos = null;
            //Inicializa as variáveis do iTextSharp utilizada
            PdfReader reader = null;
            Document doc = null;
            PdfCopy pdfcpy = null;
            PdfImportedPage page = null;

            int pagecount = 0;
            string outfile = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(DestinationPath)) { DestinationPath = System.IO.Path.GetTempPath(); };

                var r = new iText.Kernel.Pdf.PdfReader(new FileInfo(pFileSource));

                reader = new PdfReader(pFileSource);


                pagecount = reader.NumberOfPages;

                int currentpage = 1;
                string ext = System.IO.Path.GetExtension(pFileSource);

                for (int i = 1; i <= pagecount; i++)
                {

                    //Abr e o PDF atual
                    doc = new Document(reader.GetPageSizeWithRotation(currentpage));

                    //Cria um Stream para receber a copia da pagina
                    outfile = System.IO.Path.GetTempFileName() + "_PAG." + i.ToString("000") + ".PDF";

                    var newPDF = new FileStream(DestinationPath + "\\" + System.IO.Path.GetFileName(outfile), FileMode.Create);

                    //Copia a pagina atual para o novo arquivo criado
                    pdfcpy = new PdfCopy(doc, newPDF);
                    doc.Open();

                    //Ao novo arquivo criado, copia uma pagina do arquivo atual
                    page = pdfcpy.GetImportedPage(reader, currentpage);
                    pdfcpy.AddPage(page);

                    currentpage += 1;
                    doc.Close();
                }
                //Finalizou, fecha o arquivo PDF
                reader.Close();
                vArquivos = Directory.GetFiles(DestinationPath);
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }
            return vArquivos;
        }

        public static List<PDFDoc> GetPDFDocFromPages(dynamic source, string tempPasta)
        {
            //Inicializa as variáveis do iTextSharp utilizada
            PdfReader reader = null;
            List<PDFDoc> listDocs = new List<PDFDoc>();

            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_hhmmss");

            Directory.CreateDirectory(tempPasta);

            try
            {
                reader = new PdfReader(source);

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    //--------------- CRIA UM ARQUIVO TEMPORARIO --------------
                    var doc = new Document();

                    var pdfFullPath = System.IO.Path.Combine(tempPasta, $"{i.ToString("000")}.{timeStamp}.PDF");

                    var newPDF = new FileStream(pdfFullPath, FileMode.Create);

                    var pdfcpy = new PdfCopy(doc, newPDF);

                    doc.AddAuthor("Arms FW");
                    doc.AddCreator("Arms FW PDF Extractor");
                    doc.AddCreationDate();
                    doc.AddKeywords($"PG|{i}, Arquivo|{pdfFullPath}, UltimaPagina|{i == reader.NumberOfPages}");

                    doc.Open();
                    //Ao novo arquivo criado, copia uma pagina do arquivo atual
                    var page = pdfcpy.GetImportedPage(reader, i);
                    pdfcpy.AddPage(page);
                    doc.Close();

                    listDocs.Add(new PDFDoc(pdfFullPath, false));
                }
                //Finalizou, fecha o arquivo PDF
                reader.Close();
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }
            return listDocs;
        }
    }
}
