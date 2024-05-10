using BitMiracle.Docotic.Pdf;
using System.IO;

namespace ArmsFW.Services.PDF
{
    public static class PDFDocoticService
    {

        public static string GetTextWithDocotic(string PDFinput, int pagina) => GetTextFromFileName(PDFinput, pagina);
        public static string GetTextWithDocotic(Stream PDFinput, int pagina) => GetTextWithDocoticFromStream(PDFinput, pagina);

        /// <summary>
        /// Obtem o conteudo do PFF
        /// </summary>
        /// <param name="PDFinput">Stream contendo o conteudo em Memoria</param>
        /// <returns></returns>
        private static string GetTextWithDocoticFromStream(Stream PDFinput, int pagina)
        {

            var pdfOtions = PdfConfigurationOptions.Create();

            //CRIA UMA INSTANCIA DO OBJETO 'PdfDocument'
            using (PdfDocument pdf = new PdfDocument(PDFinput))
            {
                PdfTextExtractionOptions opcoes = new PdfTextExtractionOptions() { WithFormatting = true, SkipInvisibleText = true };

                if (pagina == 0)
                {
                    return pdf.GetTextWithFormatting();//(opcoes);
                }
                else
                {
                    return pdf.GetPage(pagina - 1).GetTextWithFormatting();//.GetText(opcoes);
                }

            }
        }

        private static string GetTextFromFileName(string PDFinput, int pagina)
        {

            var pdfOtions = PdfConfigurationOptions.Create();

            //CRIA UMA INSTANCIA DO OBJETO 'PdfDocument'
            using (PdfDocument pdf = new PdfDocument(PDFinput))
            {
                PdfTextExtractionOptions opcoes = new PdfTextExtractionOptions() { WithFormatting = true, SkipInvisibleText = true };

                if (pagina == 0)
                {
                    return pdf.GetTextWithFormatting();//(opcoes);
                }
                else
                {
                    return pdf.GetPage(pagina - 1).GetTextWithFormatting();//.GetText(opcoes);
                }

            }
        }


    }
}