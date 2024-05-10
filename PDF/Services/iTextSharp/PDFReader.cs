using ArmsFW.Services.Extensions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.IO;

namespace ArmsFW.Services.PDF
{
    public class PDFReader
    {
        public PdfReader Reader { get; set; }
        public PDFReader(string PdfFile)
        {
            InitializeReader(PdfFile);
        }

        private bool InitializeReader(string PdfFile)
        {
            try
            {
                Reader = new PdfReader(PdfFile);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public string GetText(int pageNumber, int streamNumber)
        {
            var strategy = new LocationTextExtractionStrategy();
            var processor = new PdfContentStreamProcessor(strategy);

            PdfDictionary resourcesDic = new PdfDictionary(PdfName.RESOURCES);


            // assuming you still need to extract the page bytes
            byte[] contents = GetContentBytesForPageStream(this.Reader, pageNumber, streamNumber);

            PdfStamper stamper = new PdfStamper(this.Reader, contents.GetStream());
            Dictionary<string, PdfLayer> layers = stamper.GetPdfLayers();

            foreach (var layer in layers)
            {
                var itemLayer = layer.Value as PdfLayer;

                var b =  itemLayer.GetBytes();
            }

            stamper.Close();

            processor.ProcessContent(contents, resourcesDic);
            return strategy.GetResultantText();
        }
        private static void GetOCGOrder(PdfArray order, PdfLayer layer)
        {
            if (!layer.OnPanel)
                return;
            
                order.Add(layer.Ref);
            List<PdfLayer> children = layer.Children;
            if (children == null)
                return;
            PdfArray kids = new PdfArray();

            
                kids.Add(new PdfString("", PdfObject.TEXT_UNICODE));

            for (int k = 0; k < children.Count; ++k)
            {
                GetOCGOrder(kids, children[k]);
            }
            if (kids.Size > 0)
                order.Add(kids);
        }
        private byte[] GetContentBytesForPageStream(PdfReader reader, int pageNumber, int streamNumber)
        {
            PdfDictionary pageDictionary = reader.GetPageN(pageNumber);
            PdfObject contentObject = pageDictionary.Get(PdfName.CONTENTS);
            if (contentObject == null)
                return new byte[0];

            byte[] contentBytes = GetContentBytesFromContentObject(contentObject, streamNumber);
            return contentBytes;
        }
        public byte[] GetContentBytesFromContentObject(PdfObject contentObject, int streamNumber)
        {
            // copy-paste logic from
            return ContentByteUtils.GetContentBytesFromContentObject(contentObject);
            // but in case PdfObject.ARRAY: only select the streamNumber you require
        }


        private static List<TextRenderInfo> GetAllTextInfor(string inPutPDF, PdfReader pdfReader)
        {
            List<TextRenderInfo> listTextInfor = new List<TextRenderInfo>();

            TextExtractionStrategy allTextInfo = new TextExtractionStrategy();

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                PdfTextExtractor.GetTextFromPage(pdfReader, i, allTextInfo);
            }
            listTextInfor = allTextInfo.textList;
            return listTextInfor;
        }
    }

    public class TextExtractionStrategy : ITextExtractionStrategy
    {
        public List<TextRenderInfo> textList = new List<TextRenderInfo>();
        public void BeginTextBlock()
        {

        }

        public void EndTextBlock()
        {

        }

        public string GetResultantText()
        {
            return "";
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            var a = renderInfo;
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            textList.Add(renderInfo);
        }
    }
}
